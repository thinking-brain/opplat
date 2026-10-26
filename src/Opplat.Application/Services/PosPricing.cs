using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Services;

public sealed record PricingInput(Guid ProductId, string Name, decimal UnitPrice, decimal TaxRate, int Quantity, decimal Discount);

public sealed record PricedLine(Guid ProductId, string Name, int Quantity, decimal UnitPrice, decimal TaxRate, decimal Discount, decimal Gross);

public sealed record PricedBreakdown(decimal Rate, decimal TaxableBase, decimal TaxAmount, decimal Gross);

public sealed record PricedSale(IReadOnlyList<PricedLine> Lines, IReadOnlyList<PricedBreakdown> Breakdowns, decimal Total);

/// <summary>
/// Prices a tax-inclusive sale. The customer pays exactly the sum of line grosses; tax is extracted per rate
/// (base = gross / (1 + rate), tax = gross - base) so base + tax always equals what was charged.
/// </summary>
public static class PosPricing
{
    public static PricedSale Price(IEnumerable<PricingInput> items)
    {
        var lines = new List<PricedLine>();
        foreach (var (productId, name, unitPrice, taxRate, quantity, discount) in items)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "Quantity must be positive.");
            }

            var lineGross = Round(quantity * unitPrice);
            if (discount < 0 || discount > lineGross)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "Discount must be between zero and the line amount.");
            }

            lines.Add(new PricedLine(productId, name, quantity, unitPrice, taxRate, discount, lineGross - discount));
        }

        var breakdowns = lines
            .GroupBy(line => line.TaxRate)
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var gross = group.Sum(line => line.Gross);
                var taxableBase = Round(gross / (1 + group.Key / 100m));
                return new PricedBreakdown(group.Key, taxableBase, gross - taxableBase, gross);
            })
            .ToList();

        return new PricedSale(lines, breakdowns, breakdowns.Sum(b => b.Gross));
    }

    public static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    public static InvoiceLine ToInvoiceLine(PricedLine line)
    {
        var net = Round(line.Gross / (1 + line.TaxRate / 100m));
        return new InvoiceLine
        {
            ProductId = line.ProductId,
            Description = line.Name,
            Quantity = line.Quantity,
            UnitPrice = Round(line.UnitPrice / (1 + line.TaxRate / 100m)),
            DiscountAmount = line.Discount,
            TaxRate = line.TaxRate,
            LineTotal = net,
            GrossTotal = line.Gross
        };
    }

    public static InvoiceTaxBreakdown ToInvoiceBreakdown(PricedBreakdown breakdown)
        => new()
        {
            TaxType = TaxType.IVA,
            Rate = breakdown.Rate,
            TaxableBase = breakdown.TaxableBase,
            TaxAmount = breakdown.TaxAmount
        };
}
