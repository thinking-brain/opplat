using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

/// <summary>Generates a PDF document for a given invoice.</summary>
public interface IInvoicePdfRenderer
{
    /// <summary>
    /// Renders the invoice as a PDF byte array.
    /// Simplified invoices use a compact ticket-style layout; full invoices include
    /// issuer / recipient blocks, line items, and a tax breakdown table.
    /// </summary>
    byte[] Render(Invoice invoice);
}
