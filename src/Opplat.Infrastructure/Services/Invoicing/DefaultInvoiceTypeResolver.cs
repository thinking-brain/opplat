using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class DefaultInvoiceTypeResolver : IInvoiceTypeResolver
{
    public InvoiceType Resolve(TenantFiscalSettings settings, Invoice invoice)
    {
        if (invoice.CustomerSnapshot.IsFinalConsumer || string.IsNullOrWhiteSpace(invoice.CustomerSnapshot.TaxId))
        {
            return invoice.TotalAmount <= settings.SimplifiedInvoiceThreshold
                ? InvoiceType.Simplified
                : InvoiceType.Full;
        }

        return InvoiceType.Full;
    }
}