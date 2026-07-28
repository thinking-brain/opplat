using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

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
        var dbContext = scope.ServiceProvider.GetRequiredService<OpplatDbContext>();

        // Dequeue up to BatchSize eligible rows using SKIP LOCKED so concurrent
        // worker replicas never race on the same record.
        var pendingIds = await dbContext.InvoiceFiscalRecords
            .FromSqlRaw(
                """
                SELECT * FROM "InvoiceFiscalRecords"
                WHERE "SubmissionStatus" = {0}
                  AND ("NextRetryAtUtc" IS NULL OR "NextRetryAtUtc" <= NOW())
                ORDER BY "CreatedAt"
                LIMIT {1}
                FOR UPDATE SKIP LOCKED
                """,
                (int)FiscalSubmissionStatus.Pending,
                _opts.BatchSize)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        if (pendingIds.Count == 0)
            return;

        logger.LogDebug("Processing {Count} fiscal record(s)", pendingIds.Count);

        foreach (var id in pendingIds)
        {
            var record = await dbContext.InvoiceFiscalRecords.FindAsync([id], cancellationToken);
            if (record is null)
                continue;

            await SubmitRecordAsync(record, dbContext, cancellationToken);
        }
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
