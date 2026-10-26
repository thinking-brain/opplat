using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Pos;
using Opplat.Domain.Entities.Sales;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Sales.Pos;

public sealed record ListRegistersQuery() : IQuery<IReadOnlyList<RegisterDto>>;

public sealed class ListRegistersQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<ListRegistersQuery, IReadOnlyList<RegisterDto>>
{
    public async Task<IReadOnlyList<RegisterDto>> Handle(ListRegistersQuery request, CancellationToken cancellationToken)
        => await dbContext.PosRegisters
            .OrderBy(register => register.Code)
            .Select(register => new RegisterDto(register.Id, register.Code, register.Name, register.InvoiceSeries, register.IsActive, register.StorageId))
            .ToListAsync(cancellationToken);
}

public sealed record CreateRegisterCommand(string Code, string Name, string InvoiceSeries, Guid? StorageId, string? User)
    : ICommand<PosResult<RegisterDto>>;

public sealed class CreateRegisterCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CreateRegisterCommand, PosResult<RegisterDto>>
{
    public async Task<PosResult<RegisterDto>> Handle(CreateRegisterCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        var series = request.InvoiceSeries.Trim();
        if (code.Length == 0 || request.Name.Trim().Length == 0 || series.Length == 0)
        {
            return PosResult<RegisterDto>.Invalid("Code, name and invoice series are required.");
        }

        if (await dbContext.PosRegisters.AnyAsync(register => register.Code == code || register.InvoiceSeries == series, cancellationToken))
        {
            return PosResult<RegisterDto>.Conflict("A register with this code or invoice series already exists.");
        }

        var register = new PosRegister
        {
            Code = code,
            Name = request.Name.Trim(),
            InvoiceSeries = series,
            StorageId = request.StorageId,
            CreatedBy = request.User
        };
        dbContext.PosRegisters.Add(register);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PosResult<RegisterDto>.Ok(new RegisterDto(register.Id, register.Code, register.Name, register.InvoiceSeries, register.IsActive, register.StorageId));
    }
}

public sealed record ListTerminalsQuery() : IQuery<IReadOnlyList<TerminalDto>>;

public sealed class ListTerminalsQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<ListTerminalsQuery, IReadOnlyList<TerminalDto>>
{
    public async Task<IReadOnlyList<TerminalDto>> Handle(ListTerminalsQuery request, CancellationToken cancellationToken)
        => await dbContext.PaymentTerminals
            .OrderBy(terminal => terminal.DisplayName)
            .Select(terminal => new TerminalDto(terminal.Id, terminal.RegisterId, terminal.Provider, terminal.ProviderTerminalId, terminal.DisplayName, terminal.IsActive))
            .ToListAsync(cancellationToken);
}

public sealed record ListPaymentProvidersQuery() : IQuery<IReadOnlyList<string>>;

public sealed class ListPaymentProvidersQueryHandler(IPaymentTerminalProviderRegistry registry)
    : IQueryHandler<ListPaymentProvidersQuery, IReadOnlyList<string>>
{
    public Task<IReadOnlyList<string>> Handle(ListPaymentProvidersQuery request, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<string>>(registry.Keys.Order().ToList());
}

public sealed record CreateTerminalCommand(Guid? RegisterId, string Provider, string ProviderTerminalId, string DisplayName, string? User)
    : ICommand<PosResult<TerminalDto>>;

public sealed class CreateTerminalCommandHandler(OpplatDbContext dbContext, IPaymentTerminalProviderRegistry registry)
    : ICommandHandler<CreateTerminalCommand, PosResult<TerminalDto>>
{
    public async Task<PosResult<TerminalDto>> Handle(CreateTerminalCommand request, CancellationToken cancellationToken)
    {
        var provider = request.Provider.Trim().ToLowerInvariant();
        if (registry.Find(provider) is null)
        {
            return PosResult<TerminalDto>.Invalid($"Unknown payment provider '{request.Provider}'.");
        }

        if (string.IsNullOrWhiteSpace(request.ProviderTerminalId) || string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return PosResult<TerminalDto>.Invalid("Terminal id and display name are required.");
        }

        var terminalId = request.ProviderTerminalId.Trim();
        if (await dbContext.PaymentTerminals.AnyAsync(t => t.Provider == provider && t.ProviderTerminalId == terminalId, cancellationToken))
        {
            return PosResult<TerminalDto>.Conflict("This terminal is already registered.");
        }

        var terminal = new PaymentTerminal
        {
            RegisterId = request.RegisterId,
            Provider = provider,
            ProviderTerminalId = terminalId,
            DisplayName = request.DisplayName.Trim(),
            CreatedBy = request.User
        };
        dbContext.PaymentTerminals.Add(terminal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PosResult<TerminalDto>.Ok(new TerminalDto(terminal.Id, terminal.RegisterId, terminal.Provider, terminal.ProviderTerminalId, terminal.DisplayName, terminal.IsActive));
    }
}

public sealed record DeactivateTerminalCommand(Guid Id) : ICommand<PosResult<bool>>;

public sealed class DeactivateTerminalCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<DeactivateTerminalCommand, PosResult<bool>>
{
    public async Task<PosResult<bool>> Handle(DeactivateTerminalCommand request, CancellationToken cancellationToken)
    {
        var terminal = await dbContext.PaymentTerminals.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (terminal is null)
        {
            return PosResult<bool>.NotFound();
        }

        terminal.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<bool>.Ok(true);
    }
}
