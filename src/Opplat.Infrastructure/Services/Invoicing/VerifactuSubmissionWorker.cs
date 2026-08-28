using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Services;

namespace Opplat.Infrastructure.Services.Invoicing;

/// <summary>
/// Transactional outbox worker that dequeues <see cref="InvoiceFiscalRecord"/> rows
/// with <see cref="FiscalSubmissionStatus.Pending"/> and submits them to the AEAT
/// Verifactu endpoint asynchronously with exponential back-off.
///
/// Uses PostgreSQL <c>SELECT … FOR UPDATE SKIP LOCKED</c> so that multiple replicas
/// can run concurrently without processing the same row twice.
/// </summary>
public sealed class VerifactuSubmissionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<VerifactuWorkerOptions> options,
    ILogger<VerifactuSubmissionWorker> logger)
    : BackgroundService
{
    private readonly VerifactuWorkerOptions _opts = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "VerifactuSubmissionWorker started (interval={Interval}, batch={Batch}, maxRetries={MaxRetries})",
            _opts.Interval, _opts.BatchSize, _opts.MaxRetries);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in VerifactuSubmissionWorker tick");
            }

            await Task.Delay(_opts.Interval, stoppingToken);
        }

        logger.LogInformation("VerifactuSubmissionWorker stopped");
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var tenantStore = scope.ServiceProvider.GetService<IMultiTenantStore<AppTenantInfo>>();
        var provisioningService = scope.ServiceProvider.GetService<TenantProvisioningService>();

        if (tenantStore is null)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OpplatDbContext>();
            await ProcessTenantBatchAsync(null, cancellationToken);
            return;
        }

        var tenants = (await tenantStore.GetAllAsync())
            .Where(tenant => tenant.IsActive)
            .ToList();

        foreach (var tenant in tenants)
        {
            try
            {
                if (provisioningService is not null)
                {
                    await provisioningService.ProvisionTenantAsync(tenant);
                }

                await ProcessTenantBatchAsync(tenant, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process Verifactu outbox for tenant {Tenant}", tenant.Identifier);
            }
        }
    }

    private async Task ProcessTenantBatchAsync(
        AppTenantInfo? tenantInfo,
        CancellationToken cancellationToken)
    {
        await using var tenantScope = scopeFactory.CreateAsyncScope();
        var tenantAccessor = tenantScope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>();
        if (tenantInfo is not null && tenantAccessor is IMultiTenantContextSetter setter)
        {
            setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);
        }

        var dbContext = tenantScope.ServiceProvider.GetRequiredService<OpplatDbContext>();
        var pendingIds = await GetPendingRecordIdsAsync(dbContext, cancellationToken);

        if (pendingIds.Count == 0)
            return;

        logger.LogDebug(
            "Processing {Count} fiscal record(s){TenantSuffix}",
            pendingIds.Count,
            tenantInfo is null ? string.Empty : $" for tenant {tenantInfo.Identifier}");

        foreach (var id in pendingIds)
        {
            var record = await dbContext.InvoiceFiscalRecords.FindAsync([id], cancellationToken);
            if (record is null)
                continue;

            await SubmitRecordAsync(record, dbContext, cancellationToken);
        }
    }

    private async Task<List<Guid>> GetPendingRecordIdsAsync(
        OpplatDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (dbContext.Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
        {
            try
            {
                var connection = dbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                await using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT "Id"
                    FROM "InvoiceFiscalRecords"
                    WHERE "SubmissionStatus" = @status
                      AND ("NextRetryAtUtc" IS NULL OR "NextRetryAtUtc" <= NOW())
                    ORDER BY "CreatedAt"
                    LIMIT @limit
                    FOR UPDATE SKIP LOCKED
                    """;

                var statusParameter = command.CreateParameter();
                statusParameter.ParameterName = "@status";
                statusParameter.Value = (int)FiscalSubmissionStatus.Pending;
                command.Parameters.Add(statusParameter);

                var limitParameter = command.CreateParameter();
                limitParameter.ParameterName = "@limit";
                limitParameter.Value = _opts.BatchSize;
                command.Parameters.Add(limitParameter);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var ids = new List<Guid>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    ids.Add(reader.GetGuid(0));
                }

                return ids;
            }
            catch (PostgresException ex) when (ex.SqlState == "42P01")
            {
                logger.LogInformation(
                    ex,
                    "InvoiceFiscalRecords table is not available yet; skipping Verifactu worker tick until the tenant schema is initialized.");
                return [];
            }
        }

        return await dbContext.InvoiceFiscalRecords
            .Where(r => r.SubmissionStatus == FiscalSubmissionStatus.Pending
                && (r.NextRetryAtUtc == null || r.NextRetryAtUtc <= DateTime.UtcNow))
            .OrderBy(r => r.CreatedAt)
            .Take(_opts.BatchSize)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    internal async Task SubmitRecordAsync(
        InvoiceFiscalRecord record,
        OpplatDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // Idempotency guard: if a previous attempt already received a CSV from AEAT
        // but crashed before updating the status, skip the network call.
        if (!string.IsNullOrEmpty(record.AeatCsv))
        {
            record.SubmissionStatus = FiscalSubmissionStatus.Accepted;
            record.AeatSubmittedAtUtc ??= DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Fiscal record {Id} accepted (idempotency path)", record.Id);
            return;
        }

        try
        {
            // TODO: replace with VerifactuSubmissionClient.SubmitAsync once the AEAT
            // SOAP/HTTP stub is wired in. For now, the Null/None provider sets
            // SubmissionStatus = NotApplicable so this branch is only reached by
            // records written with SubmissionMode = Verifactu.
            await SubmitToAeatAsync(record, cancellationToken);

            record.SubmissionStatus = FiscalSubmissionStatus.Accepted;
            record.AeatSubmittedAtUtc = DateTime.UtcNow;
            record.LastErrorMessage = null;
            logger.LogInformation("Fiscal record {Id} submitted and accepted", record.Id);
        }
        catch (Exception ex)
        {
            record.RetryCount += 1;
            record.LastErrorMessage = ex.Message;

            if (record.RetryCount >= _opts.MaxRetries)
            {
                record.SubmissionStatus = FiscalSubmissionStatus.Rejected;
                logger.LogWarning(
                    "Fiscal record {Id} rejected after {Retries} attempt(s): {Error}",
                    record.Id, record.RetryCount, ex.Message);
            }
            else
            {
                var backoff = TimeSpan.FromSeconds(
                    Math.Min(
                        _opts.BaseBackoffSeconds * Math.Pow(2, record.RetryCount),
                        _opts.MaxBackoff.TotalSeconds));
                record.NextRetryAtUtc = DateTime.UtcNow.Add(backoff);
                logger.LogWarning(
                    "Fiscal record {Id} failed (attempt {Retry}/{Max}), next retry at {Next}: {Error}",
                    record.Id, record.RetryCount, _opts.MaxRetries, record.NextRetryAtUtc, ex.Message);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    // TODO: implement VerifactuSubmissionClient and inject it here (Phase 3, Step 9).
    // This stub throws NotImplementedException so records submitted in Verifactu mode
    // will enter the retry loop immediately, making the outbox plumbing testable end-to-end
    // before the real AEAT SOAP client exists.
    private static Task SubmitToAeatAsync(InvoiceFiscalRecord record, CancellationToken cancellationToken)
        => throw new NotImplementedException(
            $"AEAT SOAP submission not yet implemented for record {record.Id}. " +
            "Enable only when VerifactuSubmissionClient is wired in.");
}
