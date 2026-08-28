using System.Reflection;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.UnitTest.Invoicing;

/// <summary>
/// Tests the retry/backoff/rejection/idempotency logic of <see cref="VerifactuSubmissionWorker"/>
/// by calling the internal <c>SubmitRecordAsync</c> method directly with an InMemory DbContext.
/// The SQL dequeue path (SELECT FOR UPDATE SKIP LOCKED) is not exercised here; that requires
/// a real PostgreSQL integration test.
/// </summary>
public class VerifactuWorkerOutboxTests
{
    private sealed class TestTenantContextAccessor : IMultiTenantContextAccessor<AppTenantInfo>, IMultiTenantContextAccessor, IMultiTenantContextSetter
    {
        public IMultiTenantContext<AppTenantInfo> MultiTenantContext { get; set; } = null!;

        IMultiTenantContext? IMultiTenantContextAccessor.MultiTenantContext => MultiTenantContext;

        IMultiTenantContext? IMultiTenantContextSetter.MultiTenantContext
        {
            set => MultiTenantContext = value as IMultiTenantContext<AppTenantInfo>;
        }
    }

    private sealed class TestTenantStore(AppTenantInfo tenant) : IMultiTenantStore<AppTenantInfo>
    {
        public Task<bool> AddAsync(AppTenantInfo tenantInfo) => Task.FromResult(true);

        public Task<bool> UpdateAsync(AppTenantInfo tenantInfo) => Task.FromResult(true);

        public Task<bool> RemoveAsync(string id) => Task.FromResult(true);

        public Task<IEnumerable<AppTenantInfo>> GetAllAsync() => Task.FromResult<IEnumerable<AppTenantInfo>>([tenant]);

        public Task<IEnumerable<AppTenantInfo>> GetAllAsync(int take, int skip) => Task.FromResult<IEnumerable<AppTenantInfo>>([tenant]);

        public Task<AppTenantInfo?> GetAsync(string id) => Task.FromResult<AppTenantInfo?>(tenant);

        public Task<AppTenantInfo?> GetByIdentifierAsync(string identifier) => Task.FromResult<AppTenantInfo?>(tenant);
    }
    private static OpplatDbContext CreateInMemoryContext()
    {
        var opts = new DbContextOptionsBuilder<OpplatDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new OpplatDbContext(opts, tenantAccessor: null);
    }

    private static VerifactuSubmissionWorker CreateWorker(VerifactuWorkerOptions? opts = null)
    {
        // IServiceScopeFactory is not exercised in SubmitRecordAsync, so a null-returning
        // factory is fine for these unit tests.
        var scopeFactory = new ServiceCollection()
            .BuildServiceProvider()
            .GetRequiredService<IServiceScopeFactory>();

        return new VerifactuSubmissionWorker(
            scopeFactory,
            Options.Create(opts ?? new VerifactuWorkerOptions()),
            NullLogger<VerifactuSubmissionWorker>.Instance);
    }

    private static InvoiceFiscalRecord CreatePendingRecord()
        => new()
        {
            InvoiceId = Guid.NewGuid(),
            RecordHash = "hash",
            HashInput = "input",
            GeneratedAtUtc = DateTime.UtcNow,
            SoftwareName = "Opplat",
            SoftwareVersion = "1.0",
            SubmissionMode = FiscalSubmissionMode.Verifactu,
            SubmissionStatus = FiscalSubmissionStatus.Pending
        };

    [Fact]
    public async Task ProcessBatchAsync_SetsTenantContext_BeforeProcessingRecords()
    {
        await using var db = CreateInMemoryContext();
        var tenant = new AppTenantInfo
        {
            Id = Guid.NewGuid().ToString(),
            Identifier = "demo",
            Name = "Demo",
            IsActive = true
        };

        var record = CreatePendingRecord();
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        var tenantAccessor = new TestTenantContextAccessor();
        var services = new ServiceCollection();
        services.AddSingleton<IMultiTenantContextAccessor<AppTenantInfo>>(tenantAccessor);
        services.AddScoped<OpplatDbContext>(_ => db);
        services.AddSingleton<IMultiTenantStore<AppTenantInfo>>(new TestTenantStore(tenant));

        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var worker = new VerifactuSubmissionWorker(
            scopeFactory,
            Options.Create(new VerifactuWorkerOptions { BatchSize = 10 }),
            NullLogger<VerifactuSubmissionWorker>.Instance);

        var method = typeof(VerifactuSubmissionWorker).GetMethod(
            "ProcessBatchAsync",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        await (Task)method.Invoke(worker, [CancellationToken.None])!;

        Assert.NotNull(tenantAccessor.MultiTenantContext);
        Assert.Same(tenant, tenantAccessor.MultiTenantContext!.TenantInfo);
    }

    [Fact]
    public async Task ProcessBatchAsync_ProcessesPendingFiscalRecords_WithoutThrowing()
    {
        await using var db = CreateInMemoryContext();
        var record = CreatePendingRecord();
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        var services = new ServiceCollection();
        services.AddSingleton<IMultiTenantContextAccessor<AppTenantInfo>>(new TestTenantContextAccessor());
        services.AddScoped<OpplatDbContext>(_ => db);
        services.AddSingleton<IMultiTenantStore<AppTenantInfo>>(new TestTenantStore(new AppTenantInfo
        {
            Id = Guid.NewGuid().ToString(),
            Identifier = "demo",
            Name = "Demo",
            IsActive = true
        }));
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var worker = new VerifactuSubmissionWorker(
            scopeFactory,
            Options.Create(new VerifactuWorkerOptions { BatchSize = 10 }),
            NullLogger<VerifactuSubmissionWorker>.Instance);

        var method = typeof(VerifactuSubmissionWorker).GetMethod(
            "ProcessBatchAsync",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        var task = (Task)method.Invoke(worker, [CancellationToken.None])!;
        await task;

        var updatedRecord = await db.InvoiceFiscalRecords.FindAsync(record.Id);
        Assert.NotNull(updatedRecord);
        Assert.Equal(1, updatedRecord!.RetryCount);
    }

    // ─── Idempotency ────────────────────────────────────────────────────────────

    [Fact]
    public async Task SubmitRecord_SkipsNetworkCall_WhenAeatCsvAlreadySet()
    {
        await using var db = CreateInMemoryContext();
        var record = CreatePendingRecord();
        record.AeatCsv = "CSV-ALREADY-RECEIVED";   // simulates crash after AEAT responded
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        var worker = CreateWorker();
        await worker.SubmitRecordAsync(record, db, CancellationToken.None);

        // Should accept without incrementing RetryCount
        Assert.Equal(FiscalSubmissionStatus.Accepted, record.SubmissionStatus);
        Assert.Equal(0, record.RetryCount);
        Assert.NotNull(record.AeatSubmittedAtUtc);
    }

    // ─── Retry / back-off ───────────────────────────────────────────────────────

    [Fact]
    public async Task SubmitRecord_IncrementsRetryCount_AndSetsNextRetryAtUtc_OnFailure()
    {
        await using var db = CreateInMemoryContext();
        var record = CreatePendingRecord();
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        var opts = new VerifactuWorkerOptions { MaxRetries = 10, BaseBackoffSeconds = 30 };
        var worker = CreateWorker(opts);

        // First failure
        await worker.SubmitRecordAsync(record, db, CancellationToken.None);

        Assert.Equal(FiscalSubmissionStatus.Pending, record.SubmissionStatus);
        Assert.Equal(1, record.RetryCount);
        Assert.NotNull(record.NextRetryAtUtc);
        Assert.True(record.NextRetryAtUtc > DateTime.UtcNow);
        Assert.NotNull(record.LastErrorMessage);
    }

    [Fact]
    public async Task SubmitRecord_BackoffIsExponential()
    {
        await using var db = CreateInMemoryContext();

        var opts = new VerifactuWorkerOptions
        {
            MaxRetries = 10,
            BaseBackoffSeconds = 30,
            MaxBackoff = TimeSpan.FromHours(4)
        };
        var worker = CreateWorker(opts);

        var record1 = CreatePendingRecord();
        db.InvoiceFiscalRecords.Add(record1);
        await db.SaveChangesAsync();

        // Retry 1: backoff = 30 × 2^1 = 60 s
        await worker.SubmitRecordAsync(record1, db, CancellationToken.None);
        var backoff1 = record1.NextRetryAtUtc!.Value - DateTime.UtcNow;

        var record2 = CreatePendingRecord();
        record2.RetryCount = 2;   // already failed twice
        db.InvoiceFiscalRecords.Add(record2);
        await db.SaveChangesAsync();

        // Retry 3: backoff = 30 × 2^3 = 240 s
        await worker.SubmitRecordAsync(record2, db, CancellationToken.None);
        var backoff3 = record2.NextRetryAtUtc!.Value - DateTime.UtcNow;

        Assert.True(backoff3 > backoff1, "Back-off should grow with each retry");
    }

    [Fact]
    public async Task SubmitRecord_CapsBackoffAtMaxBackoff()
    {
        await using var db = CreateInMemoryContext();

        var opts = new VerifactuWorkerOptions
        {
            MaxRetries = 20,
            BaseBackoffSeconds = 30,
            MaxBackoff = TimeSpan.FromSeconds(120)  // low cap for the test
        };
        var worker = CreateWorker(opts);

        var record = CreatePendingRecord();
        record.RetryCount = 15;  // 30 × 2^16 >> 120 s cap
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        await worker.SubmitRecordAsync(record, db, CancellationToken.None);

        var backoff = record.NextRetryAtUtc!.Value - DateTime.UtcNow;
        Assert.True(backoff.TotalSeconds <= 125, "Back-off must not exceed MaxBackoff");  // 5 s tolerance
    }

    // ─── Max retries / rejection ────────────────────────────────────────────────

    [Fact]
    public async Task SubmitRecord_MarksRejected_WhenMaxRetriesExceeded()
    {
        await using var db = CreateInMemoryContext();

        var opts = new VerifactuWorkerOptions { MaxRetries = 3 };
        var worker = CreateWorker(opts);

        var record = CreatePendingRecord();
        record.RetryCount = 2;   // next failure will be the 3rd → equal MaxRetries
        db.InvoiceFiscalRecords.Add(record);
        await db.SaveChangesAsync();

        await worker.SubmitRecordAsync(record, db, CancellationToken.None);

        Assert.Equal(FiscalSubmissionStatus.Rejected, record.SubmissionStatus);
        Assert.Equal(3, record.RetryCount);
    }
}
