using Microsoft.EntityFrameworkCore;
using Moq;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Application.Features.Invoicing.Invoices;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.UnitTest.Invoicing;

/// <summary>
/// Verifies the transactional outbox guarantee: <see cref="IssueInvoiceCommand"/> must
/// persist both the <see cref="Invoice"/> (Status = Issued) and its
/// <see cref="InvoiceFiscalRecord"/> in the same <see cref="DbContext.SaveChangesAsync"/> call.
/// </summary>
public class IssueInvoiceCommandOutboxTests
{
    private static OpplatDbContext CreateInMemoryContext()
    {
        var opts = new DbContextOptionsBuilder<OpplatDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
        return new OpplatDbContext(opts, tenantAccessor: null);
    }

    [Fact]
    public async Task IssueInvoice_WritesFiscalRecordPending_InSameSaveAsInvoice()
    {
        await using var db = CreateInMemoryContext();

        // Arrange: seed tenant fiscal settings (required by the handler)
        db.TenantFiscalSettings.Add(new TenantFiscalSettings
        {
            LegalName = "Test Tenant",
            TaxId = "B12345678",
            FiscalAddress = "Calle Test 1",
            DefaultSeries = "A",
            InvoicingMode = InvoicingMode.Verifactu,
            SimplifiedInvoiceThreshold = 400m
        });

        // Seed a draft invoice
        var invoice = new Invoice
        {
            Series = "A",
            Number = 0,
            FullNumber = string.Empty,
            IssueDate = new DateTime(2026, 7, 28, 10, 0, 0, DateTimeKind.Utc),
            Status = InvoiceStatus.Draft,
            Subtotal = 100m,
            TotalAmount = 121m,
            CustomerSnapshot = new CustomerSnapshot { Name = "Client", IsFinalConsumer = true }
        };
        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();

        // Mock services
        var counter = new InvoiceCounter { Series = "A", Year = 2026, LastNumber = 1 };
        var counterSvc = new Mock<IInvoiceCounterService>();
        counterSvc.Setup(s => s.GetNextAsync("A", 2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync(counter);

        var typeResolver = new Mock<IInvoiceTypeResolver>();
        typeResolver.Setup(r => r.Resolve(It.IsAny<TenantFiscalSettings>(), It.IsAny<Invoice>()))
            .Returns(InvoiceType.Simplified);

        // Provider simulates Verifactu mode: SubmissionStatus = Pending
        var pendingRecord = new InvoiceFiscalRecord
        {
            InvoiceId = invoice.Id,
            RecordHash = "abc123",
            HashInput = "input",
            GeneratedAtUtc = DateTime.UtcNow,
            SoftwareName = "Opplat",
            SoftwareVersion = "1.0",
            SubmissionMode = FiscalSubmissionMode.Verifactu,
            SubmissionStatus = FiscalSubmissionStatus.Pending
        };
        var fiscalProvider = new Mock<IInvoiceFiscalizationProvider>();
        fiscalProvider
            .Setup(p => p.GenerateRecordAsync(It.IsAny<Invoice>(), It.IsAny<InvoiceFiscalRecord?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingRecord);

        var handler = new IssueInvoiceCommandHandler(db, counterSvc.Object, fiscalProvider.Object, typeResolver.Object);

        // Act
        var result = await handler.Handle(new IssueInvoiceCommand(invoice.Id.ToString(), null), CancellationToken.None);

        // Assert: command succeeded
        Assert.True(result.Succeeded, result.Message);

        // Both changes must be visible after a single SaveChangesAsync
        var savedInvoice = await db.Invoices.FindAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.Issued, savedInvoice!.Status);
        Assert.Equal(1, savedInvoice.Number);

        var savedRecord = await db.InvoiceFiscalRecords.FirstOrDefaultAsync(r => r.InvoiceId == invoice.Id);
        Assert.NotNull(savedRecord);
        Assert.Equal(FiscalSubmissionStatus.Pending, savedRecord.SubmissionStatus);
        Assert.Equal(FiscalSubmissionMode.Verifactu, savedRecord.SubmissionMode);
        // Outbox fields default: no retries yet
        Assert.Equal(0, savedRecord.RetryCount);
        Assert.Null(savedRecord.NextRetryAtUtc);
        Assert.Null(savedRecord.LastErrorMessage);
    }

    [Fact]
    public async Task IssueInvoice_FailsIfAlreadyIssued()
    {
        await using var db = CreateInMemoryContext();

        db.TenantFiscalSettings.Add(new TenantFiscalSettings
        {
            LegalName = "T",
            TaxId = "B00000000",
            FiscalAddress = "A",
            DefaultSeries = "A",
            InvoicingMode = InvoicingMode.None
        });

        var invoice = new Invoice
        {
            Series = "A",
            Number = 1,
            FullNumber = "A-000001",
            IssueDate = new DateTime(2026, 7, 28, 10, 0, 0, DateTimeKind.Utc),
            Status = InvoiceStatus.Issued,
            Subtotal = 50m,
            TotalAmount = 50m,
            CustomerSnapshot = new CustomerSnapshot { Name = "C", IsFinalConsumer = true }
        };
        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();

        var handler = new IssueInvoiceCommandHandler(
            db,
            Mock.Of<IInvoiceCounterService>(),
            Mock.Of<IInvoiceFiscalizationProvider>(),
            Mock.Of<IInvoiceTypeResolver>());

        var result = await handler.Handle(new IssueInvoiceCommand(invoice.Id.ToString(), null), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains("already issued", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
