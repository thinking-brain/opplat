using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Opplat.Application.Features.Sales.Pos;
using Opplat.Application.Services;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Domain.Entities.Sales;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Services.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;
using Opplat.Infrastructure.Services.Pos;

namespace Opplat.UnitTest.Pos;

public class PosFlowTests
{
    private sealed class Harness
    {
        public required OpplatDbContext Db { get; init; }

        public required PosRegister Register { get; init; }

        public required PaymentTerminal Terminal { get; init; }

        public required ProductForSale Coffee { get; init; }

        public required ProductForSale Book { get; init; }

        public required PaymentTerminalProviderRegistry Registry { get; init; }

        public required IOptions<PosPaymentOptions> Options { get; init; }

        public required InvoiceIssuingService Issuing { get; init; }
    }

    private static async Task<Harness> CreateHarnessAsync(decimal cashLimit = 1000m)
    {
        var db = new OpplatDbContext(
            new DbContextOptionsBuilder<OpplatDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            tenantAccessor: null);

        db.TenantFiscalSettings.Add(new TenantFiscalSettings
        {
            LegalName = "Cafe SL",
            TaxId = "B12345678",
            FiscalAddress = "Calle Mayor 1",
            DefaultSeries = "A",
            InvoicingMode = InvoicingMode.Verifactu,
            SimplifiedInvoiceThreshold = 400m
        });

        var register = new PosRegister { Code = "C1", Name = "Caja 1", InvoiceSeries = "T1" };
        var terminal = new PaymentTerminal { Provider = "mock", ProviderTerminalId = "T-001", DisplayName = "Datafono 1" };
        var coffee = new ProductForSale { Name = "Cafe", Code = "CAF", Description = "Cafe", Price = 2.20m, TaxRate = 10m, Active = true };
        var book = new ProductForSale { Name = "Libro", Code = "LIB", Description = "Libro", Price = 12.10m, TaxRate = 4m, Active = true };
        db.AddRange(register, terminal, coffee, book);
        await db.SaveChangesAsync();

        var options = Options.Create(new PosPaymentOptions { MockApprovalDelay = TimeSpan.Zero, CashPaymentLimit = cashLimit });
        var registry = new PaymentTerminalProviderRegistry([new MockPaymentTerminalProvider(options, TimeProvider.System)]);
        var issuing = new InvoiceIssuingService(
            db,
            new InvoiceCounterService(db),
            new VerifactuFiscalizationProvider(),
            new DefaultInvoiceTypeResolver());

        return new Harness { Db = db, Register = register, Terminal = terminal, Coffee = coffee, Book = book, Registry = registry, Options = options, Issuing = issuing };
    }

    private static async Task OpenShiftAsync(Harness h)
        => Assert.True((await new OpenShiftCommandHandler(h.Db).Handle(new OpenShiftCommand(h.Register.Id, 50m, "ana"), default)).Succeeded);

    private static async Task<SaleDto> CreateSaleAsync(Harness h, params (ProductForSale Product, int Quantity)[] lines)
    {
        var result = await new CreateSaleCommandHandler(h.Db).Handle(
            new CreateSaleCommand(new CreateSaleRequest(h.Register.Id, lines.Select(l => new SaleLineRequest(l.Product.Id, l.Quantity)).ToList()), "ana"),
            default);
        Assert.True(result.Succeeded, result.Message);
        return result.Data!;
    }

    private static Task<PosResult<PaymentDto>> PayAsync(Harness h, Guid saleId, SalePaymentMethod method, decimal? amount, string? key = null)
        => new AddPaymentCommandHandler(h.Db, h.Registry, h.Options).Handle(
            new AddPaymentCommand(saleId, new AddPaymentRequest(method, amount, method == SalePaymentMethod.Card ? h.Terminal.Id : null, key), "ana"),
            default);

    private static Task<PosResult<SaleDto>> CompleteAsync(Harness h, Guid saleId, CompleteSaleRequest? request = null)
        => new CompleteSaleCommandHandler(h.Db, h.Issuing).Handle(new CompleteSaleCommand(saleId, request, "ana"), default);

    [Fact]
    public void Pricing_ExtractsTaxFromTaxInclusivePrices()
    {
        var priced = PosPricing.Price([
            new PricingInput(Guid.NewGuid(), "A", 10m, 21m, 1, 0m),
            new PricingInput(Guid.NewGuid(), "B", 11m, 10m, 2, 2m)
        ]);

        Assert.Equal(30m, priced.Total);
        var standard = Assert.Single(priced.Breakdowns, b => b.Rate == 21m);
        Assert.Equal(8.26m, standard.TaxableBase);
        Assert.Equal(1.74m, standard.TaxAmount);
        var reduced = Assert.Single(priced.Breakdowns, b => b.Rate == 10m);
        Assert.Equal(20m, reduced.Gross);
        Assert.Equal(reduced.Gross, reduced.TaxableBase + reduced.TaxAmount);
    }

    [Fact]
    public async Task CompleteSale_WithCash_IssuesSimplifiedVerifactuInvoice()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Coffee, 2), (h.Book, 1));
        Assert.Equal(16.50m, sale.Total);

        var payment = await PayAsync(h, sale.Id, SalePaymentMethod.Cash, 20m);
        Assert.Equal(16.50m, payment.Data!.Amount);
        Assert.Equal(3.50m, payment.Data.ChangeGiven);

        var completed = await CompleteAsync(h, sale.Id);

        Assert.True(completed.Succeeded, completed.Message);
        Assert.Equal(SaleStatus.Completed, completed.Data!.Status);
        Assert.Equal("T1-000001", completed.Data.InvoiceNumber);

        var invoice = await h.Db.Invoices.Include(i => i.FiscalRecord).Include(i => i.TaxBreakdowns).SingleAsync();
        Assert.Equal(InvoiceType.Simplified, invoice.InvoiceType);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(16.50m, invoice.TotalAmount);
        Assert.Equal(invoice.TotalAmount, invoice.TaxBreakdowns.Sum(t => t.TaxableBase + t.TaxAmount));
        Assert.Equal("B12345678", invoice.FiscalRecord!.IssuerTaxId);
        Assert.Equal(FiscalSubmissionStatus.Pending, invoice.FiscalRecord.SubmissionStatus);
        Assert.Equal(1, invoice.FiscalRecord.ChainSequence);
        Assert.Contains("nif=B12345678", invoice.FiscalRecord.QrCodePayload);
    }

    [Fact]
    public async Task CompleteSale_ChainsRecordsAndNumbersSequentially()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);

        for (var i = 0; i < 3; i++)
        {
            var sale = await CreateSaleAsync(h, (h.Coffee, 1));
            await PayAsync(h, sale.Id, SalePaymentMethod.Cash, 5m);
            Assert.True((await CompleteAsync(h, sale.Id)).Succeeded);
        }

        var records = await h.Db.InvoiceFiscalRecords.Include(r => r.Invoice).OrderBy(r => r.ChainSequence).ToListAsync();
        Assert.Equal([1L, 2L, 3L], records.Select(r => r.ChainSequence));
        Assert.Equal(["T1-000001", "T1-000002", "T1-000003"], records.Select(r => r.Invoice!.FullNumber));
        Assert.Null(records[0].PreviousRecordHash);
        Assert.Equal(records[0].RecordHash, records[1].PreviousRecordHash);
        Assert.Equal(records[1].RecordHash, records[2].PreviousRecordHash);
    }

    [Fact]
    public async Task CompleteSale_IsIdempotent()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Coffee, 1));
        await PayAsync(h, sale.Id, SalePaymentMethod.Cash, 5m);

        var first = await CompleteAsync(h, sale.Id);
        var second = await CompleteAsync(h, sale.Id);

        Assert.True(second.Succeeded);
        Assert.Equal(first.Data!.InvoiceId, second.Data!.InvoiceId);
        Assert.Equal(1, await h.Db.Invoices.CountAsync());
    }

    [Fact]
    public async Task CompleteSale_WithoutFullPayment_IsRejected()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Book, 1));
        await PayAsync(h, sale.Id, SalePaymentMethod.Cash, 5m);

        var result = await CompleteAsync(h, sale.Id);

        Assert.False(result.Succeeded);
        Assert.Equal(PosErrorKind.Conflict, result.Error);
        Assert.Empty(h.Db.Invoices);
    }

    [Fact]
    public async Task CreateSale_RequiresOpenShift()
    {
        var h = await CreateHarnessAsync();

        var result = await new CreateSaleCommandHandler(h.Db).Handle(
            new CreateSaleCommand(new CreateSaleRequest(h.Register.Id, [new SaleLineRequest(h.Coffee.Id, 1)]), "ana"), default);

        Assert.False(result.Succeeded);
        Assert.Equal(PosErrorKind.Conflict, result.Error);
    }

    [Fact]
    public async Task CashPayment_AboveLegalLimit_IsRejected()
    {
        var h = await CreateHarnessAsync(cashLimit: 10m);
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Book, 1), (h.Coffee, 5));

        var result = await PayAsync(h, sale.Id, SalePaymentMethod.Cash, 30m);

        Assert.False(result.Succeeded);
        Assert.Equal(PosErrorKind.Validation, result.Error);
    }

    [Fact]
    public async Task CardPayment_ApprovedThenPolled_CompletesSale()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Book, 1));

        var started = await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "key-1");
        Assert.True(started.Succeeded, started.Message);

        var polled = await new GetPaymentQueryHandler(h.Db, h.Registry).Handle(new GetPaymentQuery(started.Data!.Id), default);
        Assert.Equal(SalePaymentStatus.Captured, polled.Data!.Status);
        Assert.Equal("4242", polled.Data.Attempt!.CardLast4);

        Assert.True((await CompleteAsync(h, sale.Id)).Succeeded);
        var invoice = await h.Db.Invoices.SingleAsync();
        Assert.Equal("Tarjeta", invoice.PaymentMethod);
    }

    [Fact]
    public async Task CardPayment_SameIdempotencyKey_DoesNotChargeTwice()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Book, 1));

        var first = await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "key-1");
        var second = await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "key-1");

        Assert.Equal(first.Data!.Id, second.Data!.Id);
        Assert.Equal(1, await h.Db.PaymentAttempts.CountAsync());
    }

    [Fact]
    public async Task CardPayment_Declined_LeavesSaleUnpaid()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        h.Book.Price = 10.01m;
        await h.Db.SaveChangesAsync();
        var sale = await CreateSaleAsync(h, (h.Book, 1));

        var started = await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "decline");
        var polled = await new GetPaymentQueryHandler(h.Db, h.Registry).Handle(new GetPaymentQuery(started.Data!.Id), default);

        Assert.Equal(SalePaymentStatus.Failed, polled.Data!.Status);
        Assert.False((await CompleteAsync(h, sale.Id)).Succeeded);
    }

    [Fact]
    public async Task CardPayment_PendingCanBeCancelled()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        h.Book.Price = 10.02m;
        await h.Db.SaveChangesAsync();
        var sale = await CreateSaleAsync(h, (h.Book, 1));

        var started = await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "hang");
        Assert.Equal(SalePaymentStatus.Pending, started.Data!.Status);

        var cancelled = await new CancelPaymentCommandHandler(h.Db, h.Registry).Handle(new CancelPaymentCommand(started.Data.Id), default);

        Assert.Equal(SalePaymentStatus.Cancelled, cancelled.Data!.Status);
        Assert.True((await new VoidSaleCommandHandler(h.Db).Handle(new VoidSaleCommand(sale.Id, "ana"), default)).Succeeded);
    }

    [Fact]
    public async Task CompleteSale_AboveSimplifiedLimit_NeedsCustomerTaxData()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var sale = await CreateSaleAsync(h, (h.Book, 40));
        await PayAsync(h, sale.Id, SalePaymentMethod.Card, null, "big");
        await new GetPaymentQueryHandler(h.Db, h.Registry).Handle(new GetPaymentQuery((await h.Db.SalePayments.SingleAsync()).Id), default);

        var rejected = await CompleteAsync(h, sale.Id);
        Assert.Equal(PosErrorKind.Validation, rejected.Error);

        var accepted = await CompleteAsync(h, sale.Id, new CompleteSaleRequest(new CustomerRequest("Acme SL", "B87654321", "Calle 2", "ES")));
        Assert.True(accepted.Succeeded, accepted.Message);
        Assert.Equal(InvoiceType.Full, (await h.Db.Invoices.SingleAsync()).InvoiceType);
    }

    [Fact]
    public async Task ShiftReport_SummarisesCashCardAndTax()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);
        var shift = await h.Db.PosShifts.SingleAsync();

        var cashSale = await CreateSaleAsync(h, (h.Coffee, 1));
        await PayAsync(h, cashSale.Id, SalePaymentMethod.Cash, 2.20m);
        await CompleteAsync(h, cashSale.Id);

        await new AddCashMovementCommandHandler(h.Db).Handle(new AddCashMovementCommand(shift.Id, PosCashMovementType.PayOut, 5m, "Proveedor", "ana"), default);

        var report = await new GetShiftReportQueryHandler(h.Db).Handle(new GetShiftReportQuery(shift.Id), default);

        Assert.Equal(1, report!.SalesCount);
        Assert.Equal(2.20m, report.TotalSales);
        Assert.Equal(50m + 2.20m - 5m, report.ExpectedCash);
        Assert.Equal(10m, Assert.Single(report.ByTaxRate).Rate);
    }

    [Fact]
    public async Task OpenShift_SecondShiftOnSameRegister_IsRejected()
    {
        var h = await CreateHarnessAsync();
        await OpenShiftAsync(h);

        var second = await new OpenShiftCommandHandler(h.Db).Handle(new OpenShiftCommand(h.Register.Id, 0m, "bob"), default);

        Assert.Equal(PosErrorKind.Conflict, second.Error);
    }
}
