using System.Security.Claims;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Admin;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Features.Admin.Queries;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Admin.Commands;
using Opplat.Domain.Models;
using Opplat.Application.Features.Account.Commands;
using Opplat.Application.Abstractions.Auth;

namespace Opplat.MainApp.Endpoints;

public static class AdminEndpoints
{
    private const string AdminCookieScheme = "AdminCookie";
    private const string AdminOidcScheme = "AdminOidc";

    public static void MapAdminEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("/admin")
            .WithTags("Admin")
            .RequireAuthorization("AdminOnly");
        var shellAwareAdminFeatures = admin.MapGroup(string.Empty)
            .AddEndpointFilter(async (context, next) =>
            {
                var authOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuthOptions>>();
                return authOptions.Value.AdminBff.ShellModeEnabled
                    ? CreateShellModeDisabledResult()
                    : await next(context);
            });

        shellAwareAdminFeatures.MapGet("/tenants",
            async (IMediator mediator) =>
            {
                var tenants = await mediator.Send(new GetTenantsQuery());
                return Results.Ok(tenants);
            })
            .WithSummary("List tenants");

        shellAwareAdminFeatures.MapPost("/tenants",
            async (UpsertTenantRequest request, IMediator mediator) =>
            {
                var tenant = await mediator.Send(new CreateTenantCommand(request));
                return tenant is null
                    ? Results.BadRequest(new { Result = false, Message = "No se pudo crear el tenant." })
                    : Results.Created($"/admin/tenants/{tenant.Identifier}", tenant);
            })
            .WithSummary("Create tenant");

        shellAwareAdminFeatures.MapPut("/tenants/{identifier}",
            async (string identifier, UpsertTenantRequest request, IMediator mediator) =>
            {
                var tenant = await mediator.Send(new UpdateTenantCommand(identifier, request));
                return tenant is null
                    ? Results.NotFound()
                    : Results.Ok(tenant);
            })
            .WithSummary("Update tenant");

        shellAwareAdminFeatures.MapDelete("/tenants/{identifier}",
            async (string identifier, IMediator mediator) =>
            {
                await mediator.Send(new DeactivateTenantCommand(identifier));
                return Results.NoContent();
            })
            .WithSummary("Deactivate tenant");

        shellAwareAdminFeatures.MapGet("/users",
            async (string? tenantIdentifier, IMediator mediator) =>
            {
                var users = await mediator.Send(new GetAdminUsersQuery(tenantIdentifier));
                return Results.Ok(users);
            })
            .WithSummary("List users across tenants");

        shellAwareAdminFeatures.MapGet("/tenants/{tenantIdentifier}/users",
            async (string tenantIdentifier, IMediator mediator, IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var users = await mediator.Send(new GetTenantUsersQuery());
                return Results.Ok(users);
            })
            .WithSummary("List tenant users");

        shellAwareAdminFeatures.MapPost("/tenants/{tenantIdentifier}/users",
            async (string tenantIdentifier, AdminCreateUserRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var user = await mediator.Send(new CreateTenantUserCommand(
                    request.Name,
                    request.LastName,
                    request.Username,
                    request.Email,
                    request.Roles));

                return user is null
                    ? Results.BadRequest(new { Result = false, Message = "No se pudo crear el usuario." })
                    : Results.Created($"/admin/tenants/{tenantIdentifier}/users/{user.UserId}", user);
            })
            .WithSummary("Create tenant user");

        shellAwareAdminFeatures.MapPut("/tenants/{tenantIdentifier}/users/{userId}",
            async (string tenantIdentifier, string userId, AdminUpdateUserRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var updated = await mediator.Send(new UpdateTenantUserCommand(
                    userId,
                    request.Name,
                    request.LastName,
                    request.Username,
                    request.Email,
                    request.Active));

                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Update tenant user metadata");

        shellAwareAdminFeatures.MapPut("/tenants/{tenantIdentifier}/users/{userId}/roles",
            async (string tenantIdentifier, string userId, AdminSetUserRolesRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var result = await mediator.Send(new ChangeRolesCommand(userId, request.Roles));
                return result.Success
                    ? Results.NoContent()
                    : Results.BadRequest(new { Result = false, Message = result.ErrorMessage });
            })
            .WithSummary("Replace tenant user roles");

        shellAwareAdminFeatures.MapPut("/tenants/{tenantIdentifier}/users/{userId}/status",
            async (string tenantIdentifier, Guid userId, AdminSetUserActiveRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var updated = await mediator.Send(new SetUserActiveStatusCommand(userId, request.Active));
                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Set tenant user active state");

        admin.MapGet("/session",
            async (HttpContext httpContext, IOptions<AuthOptions> authOptions) =>
            {
                var session = await BuildSessionAsync(httpContext, authOptions.Value);
                return Results.Ok(session);
            })
            .WithSummary("Get current admin session");

        admin.MapGet("/session/current-user",
            async (HttpContext httpContext, IOptions<AuthOptions> authOptions) =>
            {
                var session = await BuildSessionAsync(httpContext, authOptions.Value);
                return Results.Ok(session);
            })
            .WithSummary("Get current admin user");

        admin.MapGet("/session/csrf",
            (HttpContext httpContext, [FromServices] IAntiforgery antiforgery, [FromServices] IOptions<AuthOptions> authOptions) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(httpContext);
                return Results.Ok(new AdminCsrfTokenDto
                {
                    HeaderName = authOptions.Value.AdminBff.CsrfHeaderName,
                    RequestToken = tokens.RequestToken ?? string.Empty
                });
            })
            .WithSummary("Issue a CSRF token for cookie-authenticated admin requests");

        var bff = app.MapGroup("/auth/bff/admin").WithTags("Admin BFF");

        bff.MapGet("/login",
            (string? returnUrl, HttpContext httpContext, IOptions<AuthOptions> authOptions) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true &&
                    httpContext.User.IsInRole(authOptions.Value.AdminRole))
                {
                    return Results.Redirect(ResolveReturnUrl(returnUrl, httpContext, authOptions.Value.AdminBff));
                }

                var properties = new AuthenticationProperties
                {
                    RedirectUri = ResolveReturnUrl(returnUrl, httpContext, authOptions.Value.AdminBff)
                };

                return Results.Challenge(properties, [AdminOidcScheme]);
            })
            .AllowAnonymous()
            .WithSummary("Start admin BFF sign-in");

        bff.MapPost("/logout",
            async (HttpContext httpContext, IOptions<AuthOptions> authOptions) =>
            {
                await TrySignOutAdminCookieAsync(httpContext);

                return Results.Ok(new
                {
                    SignedOut = true,
                    LoginPath = authOptions.Value.AdminBff.LoginPath
                });
            })
            .WithSummary("Clear the admin BFF session");

        bff.MapGet("/access-denied",
            () => Results.Json(new
            {
                Result = false,
                Message = "The admin portal requires the SuperAdmin role."
            }, statusCode: StatusCodes.Status403Forbidden))
            .AllowAnonymous()
            .WithSummary("Admin BFF access denied");
    }

    private static IResult? ValidateResolvedTenant(
        string routeTenantIdentifier,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        var resolvedTenant = tenantAccessor.MultiTenantContext?.TenantInfo?.Identifier;
        if (string.IsNullOrWhiteSpace(resolvedTenant))
        {
            return Results.BadRequest(new
            {
                Result = false,
                Message = "The X-Tenant-Identifier header must identify the tenant for this admin request."
            });
        }

        if (!string.Equals(routeTenantIdentifier, resolvedTenant, StringComparison.OrdinalIgnoreCase))
        {
            return Results.BadRequest(new
            {
                Result = false,
                Message = "The tenant route segment must match the resolved tenant header."
            });
        }

        return null;
    }

    private static async Task<AdminSessionDto> BuildSessionAsync(HttpContext httpContext, AuthOptions authOptions)
    {
        var cookieResult = await httpContext.AuthenticateAsync(AdminCookieScheme);
        var expiresAtUtc = cookieResult.Succeeded
            ? cookieResult.Properties?.ExpiresUtc
            : TryGetTokenExpiration(httpContext.User);

        return new AdminSessionDto
        {
            IsAuthenticated = httpContext.User.Identity?.IsAuthenticated == true,
            ShellModeEnabled = authOptions.AdminBff.ShellModeEnabled,
            AuthenticationMode = ResolveAuthenticationMode(httpContext, cookieResult.Succeeded),
            ExpiresAtUtc = expiresAtUtc,
            LoginPath = authOptions.AdminBff.LoginPath,
            LogoutPath = authOptions.AdminBff.LogoutPath,
            CsrfHeaderName = authOptions.AdminBff.CsrfHeaderName,
            User = BuildSessionUser(httpContext.User)
        };
    }

    private static IResult CreateShellModeDisabledResult() =>
        Results.Json(new
        {
            Result = false,
            Message = "Admin troubleshooting shell mode is enabled. Admin data and management endpoints are temporarily unavailable."
        }, statusCode: StatusCodes.Status503ServiceUnavailable);

    private static async Task TrySignOutAdminCookieAsync(HttpContext httpContext)
    {
        var handlerProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var handler = await handlerProvider.GetHandlerAsync(httpContext, AdminCookieScheme);
        if (handler is IAuthenticationSignOutHandler)
            await httpContext.SignOutAsync(AdminCookieScheme);
    }

    private static string ResolveAuthenticationMode(HttpContext httpContext, bool hasCookieSession)
    {
        if (httpContext.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return "bearer";

        if (hasCookieSession)
            return "cookie";

        return httpContext.User.Identity?.AuthenticationType ?? "unknown";
    }

    private static DateTimeOffset? TryGetTokenExpiration(ClaimsPrincipal principal)
    {
        var exp = principal.FindFirst("exp")?.Value;
        if (!long.TryParse(exp, out var epochSeconds))
            return null;

        return DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
    }

    private static AdminSessionUserDto? BuildSessionUser(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return null;

        return new AdminSessionUserDto
        {
            UserId = principal.FindFirstValue("sub")
                     ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? string.Empty,
            Username = principal.FindFirstValue(AuthClaimTypes.PreferredUserName)
                       ?? principal.Identity?.Name
                       ?? string.Empty,
            Name = principal.FindFirstValue("given_name")
                   ?? principal.FindFirstValue("name")
                   ?? string.Empty,
            LastName = principal.FindFirstValue("family_name") ?? string.Empty,
            Email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("email")
                    ?? string.Empty,
            Roles = principal.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(role => role, StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }

    private static string ResolveReturnUrl(string? requestedReturnUrl, HttpContext httpContext, AdminBffOptions options)
    {
        var normalizedOrigins = options.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(NormalizeOrigin)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var defaultOrigin = ResolveDefaultOrigin(options, normalizedOrigins);

        if (TryResolveAbsoluteReturnUrl(requestedReturnUrl, normalizedOrigins, out var absoluteReturnUrl))
            return absoluteReturnUrl;

        if (TryResolveRelativeReturnUrl(requestedReturnUrl, normalizedOrigins, defaultOrigin, httpContext, out var relativeReturnUrl))
            return relativeReturnUrl;

        var requestOrigin = NormalizeOrigin(httpContext.Request.Headers.Origin.ToString());
        if (!string.IsNullOrWhiteSpace(requestOrigin) &&
            normalizedOrigins.Contains(requestOrigin, StringComparer.OrdinalIgnoreCase))
        {
            return $"{requestOrigin}/";
        }

        return string.IsNullOrWhiteSpace(defaultOrigin) ? "/" : $"{defaultOrigin}/";
    }

    private static bool TryResolveAbsoluteReturnUrl(string? requestedReturnUrl, IReadOnlyCollection<string> allowedOrigins, out string absoluteReturnUrl)
    {
        absoluteReturnUrl = string.Empty;
        if (!Uri.TryCreate(requestedReturnUrl, UriKind.Absolute, out var absoluteUri))
            return false;

        var origin = NormalizeOrigin(absoluteUri.GetLeftPart(UriPartial.Authority));
        if (!allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
            return false;

        absoluteReturnUrl = absoluteUri.ToString();
        return true;
    }

    private static bool TryResolveRelativeReturnUrl(
        string? requestedReturnUrl,
        IReadOnlyCollection<string> allowedOrigins,
        string defaultOrigin,
        HttpContext httpContext,
        out string relativeReturnUrl)
    {
        relativeReturnUrl = string.Empty;
        if (string.IsNullOrWhiteSpace(requestedReturnUrl) ||
            requestedReturnUrl.StartsWith("//", StringComparison.Ordinal) ||
            !Uri.TryCreate(requestedReturnUrl, UriKind.Relative, out _))
        {
            return false;
        }

        var origin = NormalizeOrigin(httpContext.Request.Headers.Origin.ToString());
        if (string.IsNullOrWhiteSpace(origin) || !allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
            origin = defaultOrigin;

        if (string.IsNullOrWhiteSpace(origin))
        {
            relativeReturnUrl = requestedReturnUrl.StartsWith('/') ? requestedReturnUrl : $"/{requestedReturnUrl}";
            return true;
        }

        relativeReturnUrl = $"{origin}{(requestedReturnUrl.StartsWith('/') ? requestedReturnUrl : $"/{requestedReturnUrl}")}";
        return true;
    }

    private static string ResolveDefaultOrigin(AdminBffOptions options, IReadOnlyCollection<string> allowedOrigins)
    {
        var configuredDefaultOrigin = NormalizeOrigin(options.DefaultOrigin);
        if (!string.IsNullOrWhiteSpace(configuredDefaultOrigin) &&
            (allowedOrigins.Count == 0 || allowedOrigins.Contains(configuredDefaultOrigin, StringComparer.OrdinalIgnoreCase)))
        {
            return configuredDefaultOrigin;
        }

        return allowedOrigins.FirstOrDefault() ?? string.Empty;
    }

    private static string NormalizeOrigin(string? origin) =>
        string.IsNullOrWhiteSpace(origin) ? string.Empty : origin.Trim().TrimEnd('/');
}
