using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Invoicing;
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

public sealed class IssueInvoiceCommandHandler(
    OpplatDbContext dbContext,
    IInvoiceIssuingService invoiceIssuingService)
    : ICommandHandler<IssueInvoiceCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceId = Guid.Parse(request.Id);
        var invoice = await dbContext.Invoices
            .Include(item => item.TaxBreakdowns)
            .Include(item => item.Lines)
            .Include(item => item.FiscalRecord)
            .FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);
        if (invoice is null)
        {
            return InvoiceCommandResult.From(false, "Entity not found.");
        }

        if (invoice.Status == InvoiceStatus.Issued)
        {
            return InvoiceCommandResult.From(false, "Invoice already issued.");
        }

        var settings = await dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            return InvoiceCommandResult.From(false, "Tenant fiscal settings are required before issuing invoices.");
        }

        var result = await invoiceIssuingService.IssueAsync(invoice, settings, cancellationToken);
        return InvoiceCommandResult.From(result.Succeeded, result.Message);
    }
}

public sealed record CancelInvoiceCommand(string Id, string? User) : ICommand<InvoiceCommandResult>;

public sealed class CancelInvoiceCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CancelInvoiceCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceId = Guid.Parse(request.Id);
        var invoice = await dbContext.Invoices
            .Include(item => item.FiscalRecord)
            .FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);
        if (invoice is null)
        {
            return InvoiceCommandResult.From(false, "Entity not found.");
        }

        // A fiscal record already exists (and may be at AEAT); flipping a flag would desync the chain.
        if (invoice.FiscalRecord is not null)
        {
            return InvoiceCommandResult.From(false, "Issued invoices cannot be cancelled; issue a rectifying invoice instead.");
        }

        invoice.Status = InvoiceStatus.Cancelled;
        await dbContext.SaveChangesAsync(cancellationToken);
        return InvoiceCommandResult.From(true, "Invoice cancelled.");
    }
}

public sealed record GetInvoicePdfQuery(string Id) : IQuery<byte[]?>;

public sealed class GetInvoicePdfQueryHandler(
    OpplatDbContext dbContext,
    IInvoicePdfRenderer pdfRenderer)
    : IQueryHandler<GetInvoicePdfQuery, byte[]?>
{
    public async Task<byte[]?> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var invoice = await dbContext.Invoices
            .Include(i => i.Lines)
            .Include(i => i.TaxBreakdowns)
            .Include(i => i.FiscalRecord)
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(request.Id), cancellationToken);

        var settings = await dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
        return invoice is null ? null : pdfRenderer.Render(invoice, settings);
    }
}

public sealed record SendInvoiceEmailCommand(string Id, string RecipientEmail, string? User)
    : ICommand<InvoiceCommandResult>;

public sealed class SendInvoiceEmailCommandHandler(
    OpplatDbContext dbContext,
    IInvoicePdfRenderer pdfRenderer,
    IInvoiceEmailService emailService)
    : ICommandHandler<SendInvoiceEmailCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(SendInvoiceEmailCommand request, CancellationToken cancellationToken)
    {
        var invoice = await dbContext.Invoices
            .Include(i => i.Lines)
            .Include(i => i.TaxBreakdowns)
            .Include(i => i.FiscalRecord)
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(request.Id), cancellationToken);

        if (invoice is null)
            return InvoiceCommandResult.From(false, "Invoice not found.");

        if (invoice.Status != InvoiceStatus.Issued)
            return InvoiceCommandResult.From(false, "Only issued invoices can be emailed.");

        var settings = await dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
        var pdfBytes = pdfRenderer.Render(invoice, settings);
        await emailService.SendAsync(invoice, pdfBytes, request.RecipientEmail, cancellationToken);

        invoice.Status = InvoiceStatus.Sent;
        await dbContext.SaveChangesAsync(cancellationToken);

        return InvoiceCommandResult.From(true, "Invoice sent by email.");
    }
}
