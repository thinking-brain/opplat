using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Services;

namespace Opplat.Api.Main.Middleware;

public class TenantValidationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
        IOptions<AuthOptions> authOptions)
    {
        if (IsTenantOptionalPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var tenantProvisioningService = context.RequestServices.GetService<TenantProvisioningService>();
        var tenantStore = context.RequestServices.GetService<IMultiTenantStore<AppTenantInfo>>();
        var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
        var claimTenantId = context.User?.FindFirst(AuthClaimTypes.TenantId)?.Value;
        var claimTenantIdentifier = context.User?.FindFirst(AuthClaimTypes.TenantIdentifier)?.Value;

        if (context.User?.Identity?.IsAuthenticated == true
            && tenantInfo is null
            && tenantStore is not null)
        {
            tenantInfo = !string.IsNullOrWhiteSpace(claimTenantIdentifier)
                ? await tenantStore.GetByIdentifierAsync(claimTenantIdentifier)
                : !string.IsNullOrWhiteSpace(claimTenantId)
                    ? await tenantStore.GetAsync(claimTenantId)
                    : null;

            if (tenantInfo is not null && tenantAccessor is IMultiTenantContextSetter setter)
                setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);
        }

        if (context.User?.Identity?.IsAuthenticated == true && tenantInfo is null)
        {
            // Global administrators don't belong to a specific tenant — let them through.
            if (context.User.IsInRole(authOptions.Value.AdminRole))
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Tenant could not be resolved. Ensure the X-Tenant-Identifier header is present or the token contains tenant claims.");
            return;
        }

        if (context.User?.Identity?.IsAuthenticated == true && tenantInfo is not null && !tenantInfo.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Tenant is inactive.");
            return;
        }

        if (tenantInfo is not null && tenantInfo.IsActive && tenantProvisioningService is not null)
            await tenantProvisioningService.ProvisionTenantAsync(tenantInfo);
        
        if (context.User?.Identity?.IsAuthenticated == true && tenantInfo != null)
        {
            // tenant_id in the JWT may contain either the GUID (from app-side enrichment)
            // or the identifier slug (from legacy Keycloak user attributes). Accept either.
            if (!string.IsNullOrWhiteSpace(claimTenantId) &&
                !string.Equals(claimTenantId, tenantInfo.Id, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(claimTenantId, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token not valid for this tenant.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(claimTenantIdentifier) &&
                !string.Equals(claimTenantIdentifier, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token not valid for this tenant.");
                return;
            }
        }
        
        await _next(context);
    }

    private static bool IsTenantOptionalPath(PathString path) =>
        path.StartsWithSegments("/auth/bff/admin", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/admin/session", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/signin-oidc-admin", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/signout-callback-oidc-admin", StringComparison.OrdinalIgnoreCase);
}
