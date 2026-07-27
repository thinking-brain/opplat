using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.UnitTest.Invoicing;

public class InvoiceTypeResolverTests
{
    [Fact]
    public void Resolve_ReturnsSimplifiedForFinalConsumerBelowThreshold()
    {
        var resolver = new DefaultInvoiceTypeResolver();
        var settings = new TenantFiscalSettings
        {
            LegalName = "Tenant",
            TaxId = "B12345678",
            FiscalAddress = "Street 1",
            DefaultSeries = "A",
            SimplifiedInvoiceThreshold = 400m
        };
        var invoice = CreateInvoice(120m, isFinalConsumer: true, taxId: null);

        var result = resolver.Resolve(settings, invoice);

        Assert.Equal(InvoiceType.Simplified, result);
    }

    [Fact]
    public void Resolve_ReturnsFullForBusinessCustomerWithTaxId()
    {
        var resolver = new DefaultInvoiceTypeResolver();
        var settings = new TenantFiscalSettings
        {
            LegalName = "Tenant",
            TaxId = "B12345678",
            FiscalAddress = "Street 1",
            DefaultSeries = "A",
            SimplifiedInvoiceThreshold = 400m
        };
        var invoice = CreateInvoice(120m, isFinalConsumer: false, taxId: "ES12345678X");

        var result = resolver.Resolve(settings, invoice);

        Assert.Equal(InvoiceType.Full, result);
    }

    private static Invoice CreateInvoice(decimal totalAmount, bool isFinalConsumer, string? taxId)
        => new()
        {
            Series = "A",
            Number = 1,
            FullNumber = "A-000001",
            IssueDate = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc),
            InvoiceType = InvoiceType.Simplified,
            TotalAmount = totalAmount,
            Subtotal = totalAmount,
            CustomerSnapshot = new CustomerSnapshot
            {
                Name = "Customer",
                TaxId = taxId,
                IsFinalConsumer = isFinalConsumer
            }
        };
}