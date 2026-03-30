using MediatR;
using Opplat.Application.Abstractions.Services;
using Opplat.Application.Dtos;
using Opplat.Domain.Models.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record CreateTenantCommand(UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record UpdateTenantCommand(string Identifier, UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record DeactivateTenantCommand(string Identifier) : IRequest;
public sealed record ProvisionTenantSchemaCommand(string Identifier) : IRequest<TenantProvisioningResult>;
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
        var id = request.Request.Id == Guid.Empty
            ? $"tenant-{identifier}"
            : request.Request.Id.ToString();

        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}

public sealed class DeactivateTenantCommandHandler(AdminTenantCatalogDbContext db) : IRequestHandler<DeactivateTenantCommand>
{
    private readonly AdminTenantCatalogDbContext _db = db;

    public async Task Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public sealed class ProvisionTenantSchemaCommandHandler(ITenantProvisioningCoordinator provisioningCoordinator) : IRequestHandler<ProvisionTenantSchemaCommand, TenantProvisioningResult>
{
    private readonly ITenantProvisioningCoordinator _provisioningCoordinator = provisioningCoordinator;

    public Task<TenantProvisioningResult> Handle(ProvisionTenantSchemaCommand request, CancellationToken cancellationToken) =>
        _provisioningCoordinator.EnsureTenantProvisionedAsync(request.Identifier, cancellationToken);
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
