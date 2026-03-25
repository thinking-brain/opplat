using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.AdminApi.Data;
using Opplat.AdminApi.Endpoints;
using Opplat.AdminApi.Models;

namespace Opplat.AdminApi.Features.Admin.Commands;

public sealed record CreateTenantCommand(UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record UpdateTenantCommand(string Identifier, UpsertTenantRequest Request) : IRequest<AdminTenantDto>;
public sealed record DeactivateTenantCommand(string Identifier) : IRequest;

public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, AdminTenantDto>
{
    private readonly AdminTenantCatalogDbContext _db;

    public CreateTenantCommandHandler(AdminTenantCatalogDbContext db)
    {
        _db = db;
    }

    public async Task<AdminTenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Request.Identifier);
        var id = string.IsNullOrWhiteSpace(request.Request.Id)
            ? $"tenant-{identifier}"
            : request.Request.Id.Trim();

        if (await _db.Tenants.AnyAsync(existing => existing.Identifier == identifier, cancellationToken))
            throw new InvalidOperationException($"A tenant with identifier '{identifier}' already exists.");

        if (await _db.Tenants.AnyAsync(existing => existing.Id == id, cancellationToken))
            throw new InvalidOperationException($"A tenant with ID '{id}' already exists.");

        var tenant = new AdminTenantInfo
        {
            Id = id,
            Identifier = identifier,
            Name = AdminPortalMappings.NormalizeTenantName(request.Request.Name),
            DatabaseName = AdminPortalMappings.NormalizeDatabaseName(request.Request.DatabaseName),
            DatabaseSchema = AdminPortalMappings.NormalizeDatabaseSchema(request.Request.DatabaseSchema),
            UserCount = 0,
            IsActive = request.Request.IsActive
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);
        return AdminPortalMappings.ToDto(tenant);
    }
}

public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, AdminTenantDto>
{
    private readonly AdminTenantCatalogDbContext _db;

    public UpdateTenantCommandHandler(AdminTenantCatalogDbContext db)
    {
        _db = db;
    }

    public async Task<AdminTenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Tenants
            .FirstOrDefaultAsync(tenant => tenant.Identifier == request.Identifier.Trim().ToLowerInvariant(), cancellationToken);

        if (existing is null)
            throw new KeyNotFoundException($"Tenant '{request.Identifier}' was not found.");

        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Request.Identifier);
        var requestedId = string.IsNullOrWhiteSpace(request.Request.Id) ? existing.Id : request.Request.Id.Trim();

        if (!string.Equals(existing.Id, requestedId, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Tenant ID cannot be changed once created.");

        if (await _db.Tenants.AnyAsync(
                tenant => tenant.Id != existing.Id && tenant.Identifier == identifier,
                cancellationToken))
        {
            throw new InvalidOperationException($"A tenant with identifier '{identifier}' already exists.");
        }

        existing.Identifier = identifier;
        existing.Name = AdminPortalMappings.NormalizeTenantName(request.Request.Name);
        existing.DatabaseName = AdminPortalMappings.NormalizeDatabaseName(request.Request.DatabaseName);
        existing.DatabaseSchema = AdminPortalMappings.NormalizeDatabaseSchema(request.Request.DatabaseSchema);
        existing.IsActive = request.Request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return AdminPortalMappings.ToDto(existing);
    }
}

public sealed class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand>
{
    private readonly AdminTenantCatalogDbContext _db;

    public DeactivateTenantCommandHandler(AdminTenantCatalogDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var identifier = AdminPortalMappings.NormalizeTenantIdentifier(request.Identifier);
        var tenant = await _db.Tenants.FirstOrDefaultAsync(existing => existing.Identifier == identifier, cancellationToken);

        if (tenant is null)
            throw new KeyNotFoundException($"Tenant '{request.Identifier}' was not found.");

        tenant.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
