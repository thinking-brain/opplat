using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Services;

/// <summary>
/// Pure in-process tax calculation: groups invoice lines by rate to produce
/// <see cref="InvoiceTaxBreakdown"/> records and computes totals.
/// </summary>
public sealed class TaxCalculationService : ITaxCalculationService
{
    public IReadOnlyList<InvoiceTaxBreakdown> CalculateBreakdowns(IEnumerable<InvoiceLine> lines)
    {
        return lines
            .GroupBy(l => (l.TaxRate, TaxType: TaxType.IVA))
            .Select(g =>
            {
                var taxableBase = g.Sum(l => l.LineTotal);
                var taxAmount = Math.Round(taxableBase * g.Key.TaxRate / 100m, 2);
                return new InvoiceTaxBreakdown
                {
                    TaxType = g.Key.TaxType,
                    Rate = g.Key.TaxRate,
                    TaxableBase = taxableBase,
                    TaxAmount = taxAmount
                };
            })
            .ToList();
    }

    public (decimal Subtotal, decimal TotalAmount) ComputeTotals(
        IEnumerable<InvoiceLine> lines,
        IEnumerable<InvoiceTaxBreakdown> breakdowns)
    {
        var subtotal = lines.Sum(l => l.LineTotal);
        var totalTax = breakdowns.Sum(b => b.TaxAmount);
        return (subtotal, subtotal + totalTax);
    }
}
