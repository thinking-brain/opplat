using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services.Billing;

namespace Opplat.UnitTest.Billing;

public sealed class MockPaymentGatewayServiceTests
{
    [Fact]
    public async Task DownloadInvoicePdfAsync_RendersTenantAndOpplatDetails()
    {
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        await using var db = new AdminTenantCatalogDbContext(options);
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Identifier = "acme",
            Name = "Acme Ltd",
            DatabaseSchema = "acme",
            DatabaseInstanceId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        var invoice = new SubscriptionInvoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            StripeInvoiceId = "inv_123",
            AmountDue = 25.50m,
            Currency = "EUR",
            Status = SubscriptionInvoiceStatus.Paid,
            PeriodStart = DateTime.UtcNow.AddDays(-30),
            PeriodEnd = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            Tenant = tenant
        };

        db.Tenants.Add(tenant);
        db.SubscriptionInvoices.Add(invoice);
        await db.SaveChangesAsync();

        var service = new MockPaymentGatewayService(
            db,
            Options.Create(new OpplatLegalDataOptions
            {
                CompanyName = "Acme Legal Ltd",
                TaxId = "B12345678",
                Address = "Calle Mayor 123, Madrid",
                Email = "billing@acme.example",
                Phone = "+34 910 000 000"
            }),
            NullLogger<MockPaymentGatewayService>.Instance);
        var pdfBytes = await service.DownloadInvoicePdfAsync("inv_123", CancellationToken.None);

        Assert.NotNull(pdfBytes);
        Assert.NotEmpty(pdfBytes);

        var content = Encoding.UTF8.GetString(pdfBytes!);
        Assert.StartsWith("%PDF-", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Mock subscription invoice", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/Type /Catalog", content, StringComparison.Ordinal);
        Assert.Contains("Acme Legal Ltd", content, StringComparison.Ordinal);
        Assert.Contains("billing@acme.example", content, StringComparison.Ordinal);
    }
}
