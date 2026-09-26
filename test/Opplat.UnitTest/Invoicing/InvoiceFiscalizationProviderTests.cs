using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;

namespace Opplat.UnitTest.Invoicing;

public class InvoiceFiscalizationProviderTests
{
    [Fact]
    public async Task GenerateRecordAsync_IsDeterministicForSameInputs()
    {
        var provider = new NullInvoiceFiscalizationProvider();
        var invoice = CreateInvoice();
        var settings = CreateSettings();

        var first = await provider.GenerateRecordAsync(invoice, settings);
        var second = await provider.GenerateRecordAsync(invoice, settings);

        Assert.Equal(first.RecordHash, second.RecordHash);
        Assert.Equal(first.HashInput, second.HashInput);
        Assert.Equal(first.QrCodePayload, second.QrCodePayload);
    }

    [Fact]
    public async Task GenerateRecordAsync_UsesPreviousHashForChainLinkage()
    {
        var provider = new NullInvoiceFiscalizationProvider();
        var invoice = CreateInvoice();
        var settings = CreateSettings();

        var previous = await provider.GenerateRecordAsync(invoice, settings);
        invoice.Number = 2;
        invoice.FullNumber = "A-000002";

        var current = await provider.GenerateRecordAsync(invoice, settings, previous);

        Assert.Equal(previous.RecordHash, current.PreviousRecordHash);
        Assert.NotEqual(previous.RecordHash, current.RecordHash);
    }

    [Fact]
    public async Task VerifactuRecord_UsesIssuerTaxIdFromTenantSettings()
    {
        var provider = new VerifactuFiscalizationProvider();
        var invoice = CreateInvoice();
        invoice.CustomerSnapshot!.TaxId = "CUSTOMER-TAX-ID";
        var settings = CreateSettings();

        var record = await provider.GenerateRecordAsync(invoice, settings);

        Assert.Contains(settings.TaxId, record.HashInput);
        Assert.DoesNotContain(invoice.CustomerSnapshot.TaxId, record.HashInput);
    }

    private static Invoice CreateInvoice()
        => new()
        {
            Series = "A",
            Number = 1,
            FullNumber = "A-000001",
            IssueDate = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc),
            InvoiceType = InvoiceType.Simplified,
            TotalAmount = 120m,
            Subtotal = 100m,
            CustomerSnapshot = new CustomerSnapshot
            {
                Name = "Customer",
                IsFinalConsumer = true
            }
        };

    private static TenantFiscalSettings CreateSettings()
        => new()
        {
            LegalName = "Issuer",
            TaxId = "B12345678",
            FiscalAddress = "Address",
            DefaultSeries = "A"
        };
}