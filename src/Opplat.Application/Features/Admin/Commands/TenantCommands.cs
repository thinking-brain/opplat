using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Abstractions.Services;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record CreateTenantCommand(UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record UpdateTenantCommand(string Identifier, UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record DeactivateTenantCommand(string Identifier) : IRequest;
public sealed record ProvisionTenantSchemaCommand(string Identifier) : IRequest<TenantProvisioningResult>;
public sealed record BulkReprovisionTenantsCommand : IRequest<BulkTenantProvisioningResult>;
public sealed record RunTenantSchemaMigrationCommand(string Identifier, TenantSchemaMigrationRequest Request) : IRequest<TenantSchemaMigrationRunReport>;
public sealed record RunBulkTenantSchemaMigrationCommand(TenantSchemaMigrationRequest Request) : IRequest<TenantSchemaMigrationRunReport>;

public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, AdminTenantDto>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator;

    public CreateTenantCommandHandler(AdminTenantCatalogDbContext db, ITenantProvisioningCoordinator provisioningCoordinator)
    {
        _db = db;
        _provisioningCoordinator = provisioningCoordinator;
    }

    public async Task<AdminTenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Request.Identifier);
        var name = AdminPortalMappings.NormalizeTenantName(request.Request.Name);
        var databaseName = AdminPortalMappings.NormalizeDatabaseName(request.Request.DatabaseName);
        var databaseSchema = !string.IsNullOrWhiteSpace(request.Request.DatabaseSchema)
            ? request.Request.DatabaseSchema.Trim()
            : $"tenant_{identifier}";

        if (await _db.Tenants.AnyAsync(t => t.Identifier == identifier, cancellationToken))
            throw new InvalidOperationException($"A tenant with identifier '{identifier}' already exists.");

        SubscriptionPlan plan;
        if (request.Request.SubscriptionPlanId.HasValue && request.Request.SubscriptionPlanId.Value != Guid.Empty)
        {
            plan = await _db.SubscriptionPlans.FindAsync([request.Request.SubscriptionPlanId.Value], cancellationToken)
                ?? throw new KeyNotFoundException($"Subscription plan '{request.Request.SubscriptionPlanId}' not found.");
        }
        else
        {
            plan = await _db.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.PricingMonthly)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("No active subscription plans are available. Please create a subscription plan first.");
        }

        var dbInstance = await _db.DatabaseInstances
            .Where(d => d.Status == DatabaseInstanceStatus.Active)
            .OrderBy(d => d.CurrentTenantSchemaCount)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No active database instances are available. Please configure a database instance first.");

        var tenant = new Tenant
        {
            Identifier = identifier,
            Name = name,
            DatabaseSchema = databaseSchema,
            Status = TenantStatus.Active,
            SubscriptionPlanId = plan.Id,
            DatabaseInstanceId = dbInstance.Id,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);

        await _provisioningCoordinator.EnsureTenantProvisionedAsync(tenant, cancellationToken);

        var created = await _db.Tenants
            .Include(t => t.TenantUsers)
            .FirstAsync(t => t.Id == tenant.Id, cancellationToken);

        return AdminPortalMappings.ToDto(created);
    }
}

public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, AdminTenantDto>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator;

    public UpdateTenantCommandHandler(AdminTenantCatalogDbContext db, ITenantProvisioningCoordinator provisioningCoordinator)
    {
        _db = db;
        _provisioningCoordinator = provisioningCoordinator;
    }

    public async Task<AdminTenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Identifier);
        var tenant = await _db.Tenants
            .Include(t => t.TenantUsers)
            .FirstOrDefaultAsync(t => t.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{identifier}' was not found.");

        tenant.Name = AdminPortalMappings.NormalizeTenantName(request.Request.Name);

        if (!string.IsNullOrWhiteSpace(request.Request.DatabaseSchema))
            tenant.DatabaseSchema = request.Request.DatabaseSchema.Trim();

        if (request.Request.SubscriptionPlanId.HasValue && request.Request.SubscriptionPlanId.Value != Guid.Empty)
        {
            var planExists = await _db.SubscriptionPlans.AnyAsync(p => p.Id == request.Request.SubscriptionPlanId.Value, cancellationToken);
            if (!planExists)
                throw new KeyNotFoundException($"Subscription plan '{request.Request.SubscriptionPlanId}' not found.");
            tenant.SubscriptionPlanId = request.Request.SubscriptionPlanId.Value;
        }

        tenant.ModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return AdminPortalMappings.ToDto(tenant);
    }
}

public sealed class DeactivateTenantCommandHandler(
    AdminTenantCatalogDbContext db,
    IUserManagementService userManagementService,
    ILogger<DeactivateTenantCommandHandler> logger) : IRequestHandler<DeactivateTenantCommand>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<DeactivateTenantCommandHandler> _logger = logger;

    public async Task Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Identifier);
        var tenant = await _db.Tenants
            .Include(t => t.TenantUsers)
            .FirstOrDefaultAsync(t => t.Identifier == identifier, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant '{identifier}' was not found.");

        if (tenant.Status == TenantStatus.Inactive)
            return; // Idempotent

        tenant.Status = TenantStatus.Inactive;
        tenant.InactivatedAt = DateTime.UtcNow;
        tenant.ModifiedAt = DateTime.UtcNow;

        foreach (var user in tenant.TenantUsers.Where(u => u.IsActive))
        {
            if (!string.IsNullOrWhiteSpace(user.EntraOid))
            {
                var result = await _userManagementService.DisableUserAsync(user.EntraOid, cancellationToken);
                if (!result.Succeeded)
                    _logger.LogWarning(
                        "Failed to disable Entra user {Oid} for tenant {Identifier}: {Error}",
                        user.EntraOid, identifier, result.Error);
            }

            user.IsActive = false;
            user.DeactivatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ProvisionTenantSchemaCommandHandler(ITenantProvisioningCoordinator provisioningCoordinator) : IRequestHandler<ProvisionTenantSchemaCommand, TenantProvisioningResult>
{
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator = provisioningCoordinator;

    public Task<TenantProvisioningResult> Handle(ProvisionTenantSchemaCommand request, CancellationToken cancellationToken) =>
        _provisioningCoordinator.EnsureTenantProvisionedAsync(request.Identifier, cancellationToken);
}

public sealed class BulkReprovisionTenantsCommandHandler : IRequestHandler<BulkReprovisionTenantsCommand, BulkTenantProvisioningResult>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator;
    private readonly ILogger<BulkReprovisionTenantsCommandHandler> _logger;

    public BulkReprovisionTenantsCommandHandler(
        AdminTenantCatalogDbContext db,
        ITenantProvisioningCoordinator provisioningCoordinator,
        ILogger<BulkReprovisionTenantsCommandHandler> logger)
    {
        _db = db;
        _provisioningCoordinator = provisioningCoordinator;
        _logger = logger;
    }

    public async Task<BulkTenantProvisioningResult> Handle(BulkReprovisionTenantsCommand request, CancellationToken cancellationToken)
    {
        var tenants = await _db.Tenants
            .AsNoTracking()
            .Where(t => t.Status == TenantStatus.Active && !string.IsNullOrEmpty(t.DatabaseSchema))
            .ToListAsync(cancellationToken);

        var results = new List<TenantProvisioningResult>(tenants.Count);

        foreach (var tenant in tenants)
        {
            try
            {
                var result = await _provisioningCoordinator.EnsureTenantProvisionedAsync(tenant, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk reprovision failed for tenant '{Identifier}'.", tenant.Identifier);
                results.Add(new TenantProvisioningResult
                {
                    TenantId = tenant.Id,
                    TenantIdentifier = tenant.Identifier,
                    DatabaseSchema = tenant.DatabaseSchema ?? string.Empty,
                    Succeeded = false,
                    ErrorMessage = ex.Message,
                    ExecutedAt = DateTime.UtcNow
                });
            }
        }

        return new BulkTenantProvisioningResult
        {
            Total = results.Count,
            Succeeded = results.Count(r => r.Succeeded),
            Failed = results.Count(r => !r.Succeeded),
            Results = results
        };
    }
}

public sealed class RunTenantSchemaMigrationCommandHandler(ITenantSchemaMigrationRunner migrationRunner) : IRequestHandler<RunTenantSchemaMigrationCommand, TenantSchemaMigrationRunReport>
{
    private readonly ITenantSchemaMigrationRunner _migrationRunner = migrationRunner;

    public Task<TenantSchemaMigrationRunReport> Handle(RunTenantSchemaMigrationCommand request, CancellationToken cancellationToken) =>
        _migrationRunner.ExecuteMigrationForTenantAsync(
            request.Identifier,
            new TenantSchemaMigrationDefinition
            {
                MigrationName = request.Request.MigrationName,
                MigrationSql = request.Request.MigrationSql,
                RollbackSql = request.Request.RollbackSql
            },
            cancellationToken);
}

public sealed class RunBulkTenantSchemaMigrationCommandHandler : IRequestHandler<RunBulkTenantSchemaMigrationCommand, TenantSchemaMigrationRunReport>
{
    private readonly ITenantSchemaMigrationRunner _migrationRunner;

    public RunBulkTenantSchemaMigrationCommandHandler(ITenantSchemaMigrationRunner migrationRunner)
    {
        _migrationRunner = migrationRunner;
    }

    public Task<TenantSchemaMigrationRunReport> Handle(RunBulkTenantSchemaMigrationCommand request, CancellationToken cancellationToken) =>
        _migrationRunner.RunMigrationBulkAsync(
            new TenantSchemaMigrationDefinition
            {
                MigrationName = request.Request.MigrationName,
                MigrationSql = request.Request.MigrationSql,
                RollbackSql = request.Request.RollbackSql
            },
            request.Request.BatchSize,
            request.Request.DelayBetweenBatchesSeconds.HasValue
                ? TimeSpan.FromSeconds(request.Request.DelayBetweenBatchesSeconds.Value)
                : null,
            cancellationToken);
}
