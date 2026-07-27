using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Features.Invoicing.Common;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Invoicing.Invoices;

public sealed record GetInvoiceQuery(string Id) : IQuery<Invoice?>;

public sealed class GetInvoiceQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<GetInvoiceQuery, Invoice?>
{
    public Task<Invoice?> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        => dbContext.Invoices
            .Include(invoice => invoice.Lines)
            .Include(invoice => invoice.TaxBreakdowns)
            .Include(invoice => invoice.FiscalRecord)
            .FirstOrDefaultAsync(invoice => invoice.Id == Guid.Parse(request.Id), cancellationToken);
}

public sealed record ListInvoicesQuery() : IQuery<IReadOnlyList<Invoice>>;

public sealed class ListInvoicesQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<ListInvoicesQuery, IReadOnlyList<Invoice>>
{
    public async Task<IReadOnlyList<Invoice>> Handle(ListInvoicesQuery request, CancellationToken cancellationToken)
        => await dbContext.Invoices
            .Include(invoice => invoice.Lines)
            .Include(invoice => invoice.FiscalRecord)
            .OrderByDescending(invoice => invoice.IssueDate)
            .ToListAsync(cancellationToken);
}

public sealed record CreateInvoiceCommand(Invoice Invoice, string? User) : ICommand<InvoiceCommandResult>;

public sealed class CreateInvoiceCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CreateInvoiceCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        request.Invoice.Status = InvoiceStatus.Draft;
        request.Invoice.IssueDate = request.Invoice.IssueDate == default ? DateTime.UtcNow : request.Invoice.IssueDate;

        await dbContext.Invoices.AddAsync(request.Invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return InvoiceCommandResult.From(true, "Invoice created.");
    }
}

public sealed record IssueInvoiceCommand(string Id, string? User) : ICommand<InvoiceCommandResult>;

public sealed class IssueInvoiceCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<IssueInvoiceCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceId = Guid.Parse(request.Id);
        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);
        if (invoice is null)
        {
            return InvoiceCommandResult.From(false, "Entity not found.");
        }

        invoice.Status = InvoiceStatus.Issued;
        invoice.IssueDate = invoice.IssueDate == default ? DateTime.UtcNow : invoice.IssueDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return InvoiceCommandResult.From(true, "Invoice issued.");
    }
}

public sealed record CancelInvoiceCommand(string Id, string? User) : ICommand<InvoiceCommandResult>;

public sealed class CancelInvoiceCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CancelInvoiceCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceId = Guid.Parse(request.Id);
        var invoice = await dbContext.Invoices.FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);
        if (invoice is null)
        {
            return InvoiceCommandResult.From(false, "Entity not found.");
        }

        invoice.Status = InvoiceStatus.Cancelled;
        await dbContext.SaveChangesAsync(cancellationToken);
        return InvoiceCommandResult.From(true, "Invoice cancelled.");
    }
}