using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public interface ITaxCalculationService
{
    /// <summary>
    /// Groups invoice lines by tax rate and builds a <see cref="InvoiceTaxBreakdown"/>
    /// record for each distinct rate found.
    /// </summary>
    IReadOnlyList<InvoiceTaxBreakdown> CalculateBreakdowns(IEnumerable<InvoiceLine> lines);

    /// <summary>
    /// Returns the subtotal (sum of line totals before tax) and the grand total
    /// (subtotal plus the sum of all tax amounts).
    /// </summary>
    (decimal Subtotal, decimal TotalAmount) ComputeTotals(
        IEnumerable<InvoiceLine> lines,
        IEnumerable<InvoiceTaxBreakdown> breakdowns);
}
