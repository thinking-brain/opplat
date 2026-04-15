using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services;

public interface ITenantProvisioningReporter
{
    Task ReportAsync(TenantProvisioningResult result, CancellationToken cancellationToken);
}

public sealed class TenantProvisioningCoordinator : ITenantProvisioningCoordinator
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly TenantSchemaProvisioningService _tenantSchemaProvisioningService;
    private readonly DatabaseInstanceAutoScalingService _databaseInstanceAutoScalingService;
    private readonly IEnumerable<ITenantProvisioningReporter> _reporters;
    private readonly ILogger<TenantProvisioningCoordinator> _logger;

    public TenantProvisioningCoordinator(
        AdminTenantCatalogDbContext db,
        TenantSchemaProvisioningService tenantSchemaProvisioningService,
        DatabaseInstanceAutoScalingService databaseInstanceAutoScalingService,
        IEnumerable<ITenantProvisioningReporter> reporters,
        ILogger<TenantProvisioningCoordinator> logger)
    {
        _db = db;
        _tenantSchemaProvisioningService = tenantSchemaProvisioningService;
        _databaseInstanceAutoScalingService = databaseInstanceAutoScalingService;
        _reporters = reporters;
        _logger = logger;
    }

    public async Task<TenantProvisioningResult> EnsureTenantProvisionedAsync(
        string tenantIdentifier,
        CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = tenantIdentifier?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedIdentifier))
            throw new ArgumentException("Tenant identifier is required.", nameof(tenantIdentifier));

        var tenant = await _db.Tenants
            .Include(model => model.DatabaseInstance)
            .FirstOrDefaultAsync(model => model.Identifier == normalizedIdentifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{tenantIdentifier}' was not found.");

        return await EnsureTenantProvisionedAsync(tenant, cancellationToken);
    }

    public async Task<TenantProvisioningResult> EnsureTenantProvisionedAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        var trackedTenant = await _db.Tenants
            .Include(model => model.DatabaseInstance)
            .FirstOrDefaultAsync(model => model.Id == tenant.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{tenant.Identifier}' was not found.");

        var result = new TenantProvisioningResult
        {
            TenantId = trackedTenant.Id,
            TenantIdentifier = trackedTenant.Identifier,
            ExecutedAt = DateTime.UtcNow
        };

        try
        {
            trackedTenant.DatabaseSchema = NormalizeDatabaseSchema(trackedTenant.DatabaseSchema, trackedTenant.Identifier);
            if (trackedTenant.DatabaseInstance is null)
            {
                trackedTenant.DatabaseInstance = await _db.DatabaseInstances
                    .FirstOrDefaultAsync(model => model.Id == trackedTenant.DatabaseInstanceId, cancellationToken)
                    ?? throw new InvalidOperationException($"Tenant '{trackedTenant.Identifier}' has no assigned database instance.");
            }

            if (!_db.Database.IsRelational())
            {
                PopulateResult(
                    result,
                    trackedTenant.DatabaseInstance,
                    trackedTenant.DatabaseSchema,
                    succeeded: true,
                    alreadyProvisioned: true);
                await ReportAsync(result, cancellationToken);
                return result;
            }

            if (await _tenantSchemaProvisioningService.SchemaExistsAsync(trackedTenant, cancellationToken))
            {
                PopulateResult(result, trackedTenant.DatabaseInstance, trackedTenant.DatabaseSchema, succeeded: true, alreadyProvisioned: true);
                await ReportAsync(result, cancellationToken);
                return result;
            }

            var selectedInstance = await _databaseInstanceAutoScalingService.EnsureAvailableInstanceAsync(
                trackedTenant.DatabaseInstance,
                reservedSlots: 1,
                cancellationToken);

            result.InstanceReassigned = selectedInstance.Id != trackedTenant.DatabaseInstanceId;
            if (result.InstanceReassigned)
            {
                trackedTenant.DatabaseInstanceId = selectedInstance.Id;
                trackedTenant.DatabaseInstance = selectedInstance;
            }

            await _db.SaveChangesAsync(cancellationToken);
            await _databaseInstanceAutoScalingService.RecalculateTenantCountsAsync(cancellationToken);

            var outcome = await _tenantSchemaProvisioningService.ProvisionTenantSchemaAsync(trackedTenant, cancellationToken);
            PopulateResult(
                result,
                trackedTenant.DatabaseInstance!,
                trackedTenant.DatabaseSchema!,
                outcome.Succeeded,
                outcome.AlreadyProvisioned,
                outcome.DatabaseCreated,
                outcome.SchemaCreated);
        }
        catch (Exception exception)
        {
            result.Succeeded = false;
            result.ErrorMessage = exception.Message;
            _logger.LogError(
                exception,
                "Tenant provisioning failed for tenant '{TenantIdentifier}'.",
                trackedTenant.Identifier);
        }

        await ReportAsync(result, cancellationToken);
        return result;
    }

    private async Task ReportAsync(TenantProvisioningResult result, CancellationToken cancellationToken)
    {
        foreach (var reporter in _reporters)
            await reporter.ReportAsync(result, cancellationToken);
    }

    private static void PopulateResult(
        TenantProvisioningResult result,
        DatabaseInstance databaseInstance,
        string databaseSchema,
        bool succeeded,
        bool alreadyProvisioned = false,
        bool databaseCreated = false,
        bool schemaCreated = false)
    {
        result.DatabaseInstanceId = databaseInstance.Id;
        result.DatabaseInstanceIdentifier = databaseInstance.Identifier;
        result.DatabaseName = databaseInstance.DatabaseName;
        result.DatabaseSchema = databaseSchema;
        result.Succeeded = succeeded;
        result.AlreadyProvisioned = alreadyProvisioned;
        result.DatabaseCreated = databaseCreated;
        result.SchemaCreated = schemaCreated;
    }

    private static string NormalizeDatabaseSchema(string? databaseSchema, string? tenantIdentifier)
    {
        if (!string.IsNullOrWhiteSpace(databaseSchema))
            return databaseSchema.Trim();

        var normalizedIdentifier = string.IsNullOrWhiteSpace(tenantIdentifier)
            ? "default"
            : new string([.. tenantIdentifier
                .Trim()
                .ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')])
                .Trim('_');

        return $"tenant_{normalizedIdentifier}";
    }
}

