// using Finbuckle.MultiTenant.Abstractions;
// using MediatR;
// using Opplat.Application.Dtos;
// using Opplat.Domain.Models;

// namespace Opplat.Application.Features.Admin.Commands;

// public record CreateTenantCommand(UpsertTenantRequest Request) : IRequest<AdminTenantDto?>;
// public record UpdateTenantCommand(string Identifier, UpsertTenantRequest Request) : IRequest<AdminTenantDto?>;
// public record DeactivateTenantCommand(string Identifier) : IRequest<bool>;

// public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, AdminTenantDto?>
// {
//     private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;

//     public CreateTenantCommandHandler(IMultiTenantStore<AppTenantInfo> tenantStore)
//     {
//         _tenantStore = tenantStore;
//     }

//     public async Task<AdminTenantDto?> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
//     {
//         var tenant = BuildTenant(request.Request, request.Request.Id ?? $"tenant-{Guid.NewGuid():N}");
//         var created = await _tenantStore.TryAddAsync(tenant);
//         return created ? TenantCommandMappings.ToDto(tenant) : null;
//     }

//     private static AppTenantInfo BuildTenant(UpsertTenantRequest request, string tenantId) => new()
//     {
//         Id = tenantId,
//         Identifier = TenantCommandMappings.NormalizeIdentifier(request.Identifier),
//         Name = request.Name.Trim(),
//         ConnectionString = request.ConnectionString.Trim(),
//         IsActive = request.IsActive
//     };
// }

// public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, AdminTenantDto?>
// {
//     private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;

//     public UpdateTenantCommandHandler(IMultiTenantStore<AppTenantInfo> tenantStore)
//     {
//         _tenantStore = tenantStore;
//     }

//     public async Task<AdminTenantDto?> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
//     {
//         var existingTenant = (await _tenantStore.GetAllAsync())
//             .FirstOrDefault(tenant => string.Equals(tenant.Identifier, request.Identifier, StringComparison.OrdinalIgnoreCase));

//         if (existingTenant is null)
//             return null;

//         var updatedTenant = new AppTenantInfo
//         {
//             Id = existingTenant.Id,
//             Identifier = TenantCommandMappings.NormalizeIdentifier(request.Request.Identifier),
//             Name = request.Request.Name.Trim(),
//             ConnectionString = request.Request.ConnectionString.Trim(),
//             JwtSigningKey = existingTenant.JwtSigningKey,
//             IsActive = request.Request.IsActive
//         };

//         var updated = await _tenantStore.TryUpdateAsync(updatedTenant);
//         return updated ? TenantCommandMappings.ToDto(updatedTenant) : null;
//     }
// }

// public sealed class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand, bool>
// {
//     private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;

//     public DeactivateTenantCommandHandler(IMultiTenantStore<AppTenantInfo> tenantStore)
//     {
//         _tenantStore = tenantStore;
//     }

//     public async Task<bool> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
//     {
//         var tenant = (await _tenantStore.GetAllAsync())
//             .FirstOrDefault(existing => string.Equals(existing.Identifier, request.Identifier, StringComparison.OrdinalIgnoreCase));

//         if (tenant is null)
//             return false;

//         tenant.IsActive = false;
//         return await _tenantStore.TryUpdateAsync(tenant);
//     }
// }

// internal static class TenantCommandMappings
// {
//     internal static AdminTenantDto ToDto(AppTenantInfo tenant) => new()
//     {
//         Id = tenant.Id ?? string.Empty,
//         Identifier = tenant.Identifier ?? string.Empty,
//         Name = tenant.Name ?? string.Empty,
//         ConnectionString = tenant.ConnectionString ?? string.Empty,
//         IsActive = tenant.IsActive
//     };

//     internal static string NormalizeIdentifier(string identifier) => identifier.Trim().ToLowerInvariant();
// }
