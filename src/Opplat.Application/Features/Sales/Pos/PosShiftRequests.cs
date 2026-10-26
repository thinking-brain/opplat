using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Domain.Entities.Sales;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Sales.Pos;

internal static class PosShiftMapping
{
    public static ShiftDto ToDto(PosShift shift)
        => new(shift.Id, shift.RegisterId, shift.Status, shift.OpenedBy, shift.OpenedAtUtc, shift.OpeningFloat, shift.ClosedAtUtc, shift.ExpectedCash, shift.CountedCash);

    public static async Task<ShiftReportDto> BuildReportAsync(OpplatDbContext dbContext, PosShift shift, CancellationToken cancellationToken)
    {
        var sales = await dbContext.Sales
            .Include(sale => sale.Payments)
            .Where(sale => sale.ShiftId == shift.Id && sale.Status == SaleStatus.Completed)
            .ToListAsync(cancellationToken);

        var saleIds = sales.Select(sale => sale.Id).ToList();
        var breakdowns = await dbContext.InvoiceTaxBreakdowns
            .Where(b => b.Invoice != null && b.Invoice.SaleId != null && saleIds.Contains(b.Invoice.SaleId.Value))
            .ToListAsync(cancellationToken);

        var captured = sales.SelectMany(sale => sale.Payments).Where(p => p.Status == SalePaymentStatus.Captured).ToList();
        var byMethod = captured
            .GroupBy(p => p.Method)
            .Select(g => new ReportLineDto(g.Key.ToString(), g.Count(), g.Sum(p => p.Amount)))
            .ToList();
        var byTax = breakdowns
            .GroupBy(b => b.Rate)
            .OrderBy(g => g.Key)
            .Select(g => new TaxReportLineDto(g.Key, g.Sum(b => b.TaxableBase), g.Sum(b => b.TaxAmount)))
            .ToList();

        var movements = await dbContext.PosCashMovements.Where(m => m.ShiftId == shift.Id).ToListAsync(cancellationToken);
        var payIn = movements.Where(m => m.Type == PosCashMovementType.PayIn).Sum(m => m.Amount);
        var payOut = movements.Where(m => m.Type == PosCashMovementType.PayOut).Sum(m => m.Amount);
        var cashSales = captured.Where(p => p.Method == SalePaymentMethod.Cash).Sum(p => p.Amount);

        return new ShiftReportDto(
            ToDto(shift),
            sales.Count,
            sales.Sum(sale => sale.TotalAmount),
            byMethod,
            byTax,
            payIn,
            payOut,
            shift.OpeningFloat + cashSales + payIn - payOut);
    }
}

public sealed record OpenShiftCommand(Guid RegisterId, decimal OpeningFloat, string? User) : ICommand<PosResult<ShiftDto>>;

public sealed class OpenShiftCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<OpenShiftCommand, PosResult<ShiftDto>>
{
    public async Task<PosResult<ShiftDto>> Handle(OpenShiftCommand request, CancellationToken cancellationToken)
    {
        if (request.OpeningFloat < 0)
        {
            return PosResult<ShiftDto>.Invalid("Opening float cannot be negative.");
        }

        var register = await dbContext.PosRegisters.FirstOrDefaultAsync(r => r.Id == request.RegisterId, cancellationToken);
        if (register is null)
        {
            return PosResult<ShiftDto>.NotFound("Register not found.");
        }

        if (!register.IsActive)
        {
            return PosResult<ShiftDto>.Invalid("Register is inactive.");
        }

        if (await dbContext.PosShifts.AnyAsync(s => s.RegisterId == register.Id && s.Status == PosShiftStatus.Open, cancellationToken))
        {
            return PosResult<ShiftDto>.Conflict("The register already has an open shift.");
        }

        var shift = new PosShift
        {
            RegisterId = register.Id,
            OpenedBy = request.User ?? "unknown",
            OpenedAtUtc = DateTime.UtcNow,
            OpeningFloat = request.OpeningFloat,
            CreatedBy = request.User
        };
        dbContext.PosShifts.Add(shift);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PosResult<ShiftDto>.Ok(PosShiftMapping.ToDto(shift));
    }
}

public sealed record GetCurrentShiftQuery(Guid RegisterId) : IQuery<ShiftDto?>;

public sealed class GetCurrentShiftQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<GetCurrentShiftQuery, ShiftDto?>
{
    public async Task<ShiftDto?> Handle(GetCurrentShiftQuery request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.PosShifts
            .FirstOrDefaultAsync(s => s.RegisterId == request.RegisterId && s.Status == PosShiftStatus.Open, cancellationToken);
        return shift is null ? null : PosShiftMapping.ToDto(shift);
    }
}

public sealed record AddCashMovementCommand(Guid ShiftId, PosCashMovementType Type, decimal Amount, string Reason, string? User)
    : ICommand<PosResult<bool>>;

public sealed class AddCashMovementCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<AddCashMovementCommand, PosResult<bool>>
{
    public async Task<PosResult<bool>> Handle(AddCashMovementCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Reason))
        {
            return PosResult<bool>.Invalid("A positive amount and a reason are required.");
        }

        var shift = await dbContext.PosShifts.FirstOrDefaultAsync(s => s.Id == request.ShiftId, cancellationToken);
        if (shift is null)
        {
            return PosResult<bool>.NotFound("Shift not found.");
        }

        if (shift.Status != PosShiftStatus.Open)
        {
            return PosResult<bool>.Conflict("The shift is closed.");
        }

        dbContext.PosCashMovements.Add(new PosCashMovement
        {
            ShiftId = shift.Id,
            Type = request.Type,
            Amount = request.Amount,
            Reason = request.Reason.Trim(),
            User = request.User ?? "unknown",
            CreatedBy = request.User
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<bool>.Ok(true);
    }
}

public sealed record CloseShiftCommand(Guid ShiftId, decimal CountedCash, string? User) : ICommand<PosResult<ShiftReportDto>>;

public sealed class CloseShiftCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CloseShiftCommand, PosResult<ShiftReportDto>>
{
    public async Task<PosResult<ShiftReportDto>> Handle(CloseShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.PosShifts.FirstOrDefaultAsync(s => s.Id == request.ShiftId, cancellationToken);
        if (shift is null)
        {
            return PosResult<ShiftReportDto>.NotFound("Shift not found.");
        }

        if (shift.Status != PosShiftStatus.Open)
        {
            return PosResult<ShiftReportDto>.Conflict("The shift is already closed.");
        }

        if (request.CountedCash < 0)
        {
            return PosResult<ShiftReportDto>.Invalid("Counted cash cannot be negative.");
        }

        var openSales = await dbContext.Sales
            .AnyAsync(sale => sale.ShiftId == shift.Id && sale.Status == SaleStatus.Draft && sale.Payments.Any(p => p.Status != SalePaymentStatus.Failed && p.Status != SalePaymentStatus.Cancelled), cancellationToken);
        if (openSales)
        {
            return PosResult<ShiftReportDto>.Conflict("Finish or void the sales with pending payments before closing the shift.");
        }

        var report = await PosShiftMapping.BuildReportAsync(dbContext, shift, cancellationToken);
        shift.Status = PosShiftStatus.Closed;
        shift.ClosedAtUtc = DateTime.UtcNow;
        shift.ClosedBy = request.User;
        shift.ExpectedCash = report.ExpectedCash;
        shift.CountedCash = request.CountedCash;
        await dbContext.SaveChangesAsync(cancellationToken);

        return PosResult<ShiftReportDto>.Ok(report with { Shift = PosShiftMapping.ToDto(shift) });
    }
}

public sealed record GetShiftReportQuery(Guid ShiftId) : IQuery<ShiftReportDto?>;

public sealed class GetShiftReportQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<GetShiftReportQuery, ShiftReportDto?>
{
    public async Task<ShiftReportDto?> Handle(GetShiftReportQuery request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.PosShifts.FirstOrDefaultAsync(s => s.Id == request.ShiftId, cancellationToken);
        return shift is null ? null : await PosShiftMapping.BuildReportAsync(dbContext, shift, cancellationToken);
    }
}
