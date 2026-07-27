using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public interface IInvoiceTypeResolver
{
    InvoiceType Resolve(TenantFiscalSettings settings, Invoice invoice);
}