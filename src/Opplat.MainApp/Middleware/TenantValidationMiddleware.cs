using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Services;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Services;

namespace Opplat.MainApp.Middleware;

public class TenantValidationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, 
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        if (IsTenantOptionalPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var tenantProvisioningService = context.RequestServices.GetService<TenantProvisioningService>();
        var tenantStore = context.RequestServices.GetService<TenantCatalogStore>();
        var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
        var claimTenantId = context.User?.FindFirst(AuthClaimTypes.TenantId)?.Value;
        var claimTenantIdentifier = context.User?.FindFirst(AuthClaimTypes.TenantIdentifier)?.Value;

        if (context.User?.Identity?.IsAuthenticated == true
            && tenantInfo is null
            && tenantStore is not null)
        {
            tenantInfo = !string.IsNullOrWhiteSpace(claimTenantIdentifier)
                ? await tenantStore.TryGetByIdentifierAsync(claimTenantIdentifier)
                : !string.IsNullOrWhiteSpace(claimTenantId)
                    ? await tenantStore.TryGetAsync(claimTenantId)
                    : null;
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
            if (!string.IsNullOrWhiteSpace(claimTenantId) && !string.Equals(claimTenantId, tenantInfo.Id, StringComparison.OrdinalIgnoreCase))
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
