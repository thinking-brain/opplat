using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.UnitTest.Invoicing;

public class InvoiceFiscalizationProviderTests
{
    [Fact]
    public async Task GenerateRecordAsync_IsDeterministicForSameInputs()
    {
        var provider = new NullInvoiceFiscalizationProvider();
        var invoice = CreateInvoice();

        var first = await provider.GenerateRecordAsync(invoice);
        var second = await provider.GenerateRecordAsync(invoice);

        Assert.Equal(first.RecordHash, second.RecordHash);
        Assert.Equal(first.HashInput, second.HashInput);
        Assert.Equal(first.QrCodePayload, second.QrCodePayload);
    }

    [Fact]
    public async Task GenerateRecordAsync_UsesPreviousHashForChainLinkage()
    {
        var provider = new NullInvoiceFiscalizationProvider();
        var invoice = CreateInvoice();

        var previous = await provider.GenerateRecordAsync(invoice);
        invoice.Number = 2;
        invoice.FullNumber = "A-000002";

        var current = await provider.GenerateRecordAsync(invoice, previous);

        Assert.Equal(previous.RecordHash, current.PreviousRecordHash);
        Assert.NotEqual(previous.RecordHash, current.RecordHash);
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
}