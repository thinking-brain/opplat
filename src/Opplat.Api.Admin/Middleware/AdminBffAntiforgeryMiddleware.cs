using Microsoft.AspNetCore.Antiforgery;

namespace Opplat.Api.Admin.Middleware;

public sealed class AdminBffAntiforgeryMiddleware
{
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Get,
        HttpMethods.Head,
        HttpMethods.Options,
        HttpMethods.Trace
    };

    private readonly RequestDelegate _next;

    public AdminBffAntiforgeryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        if (!RequiresValidation(context))
        {
            await _next(context);
            return;
        }

        try
        {
            await antiforgery.ValidateRequestAsync(context);
        }
        catch (AntiforgeryValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                Result = false,
                Message = "A valid admin CSRF token is required for cookie-authenticated state changes."
            });
            return;
        }

        await _next(context);
    }

    private static bool RequiresValidation(HttpContext context)
    {
        if (SafeMethods.Contains(context.Request.Method))
            return false;

        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return false;

        if (context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return false;

        return context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase)
               || context.Request.Path.StartsWithSegments("/auth/bff/admin/logout", StringComparison.OrdinalIgnoreCase);
    }
}
