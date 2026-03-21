using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using Opplat.MainApp.Auth;
using Opplat.MainApp.Models;
using System.Threading.Tasks;

namespace Opplat.MainApp.Middleware;

public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    
    public TenantValidationMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context, 
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
        var claimTenantId = context.User?.FindFirst(AuthClaimTypes.TenantId)?.Value;
        var claimTenantIdentifier = context.User?.FindFirst(AuthClaimTypes.TenantIdentifier)?.Value;
        
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
}
