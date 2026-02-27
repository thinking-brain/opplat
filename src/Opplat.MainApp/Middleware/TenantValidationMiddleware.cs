using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
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
        var claimTenantId = context.User?.FindFirst("tenant_id")?.Value;
        
        if (context.User?.Identity?.IsAuthenticated == true 
            && claimTenantId != null 
            && tenantInfo != null
            && claimTenantId != tenantInfo.Id)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Token not valid for this tenant.");
            return;
        }
        
        await _next(context);
    }
}
