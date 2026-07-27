using System.Security.Claims;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Admin;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Admin.Commands;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Features.Admin.Queries;
using Opplat.Application.Abstractions.Auth;

namespace Opplat.Api.Admin.Endpoints;

public static class AdminEndpoints
{
    private const string AdminCookieScheme = "AdminCookie";
    private const string AdminOidcScheme = "AdminOidc";

    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminData = app.MapGroup("/admin")
            .WithTags("Admin");

        adminData.MapGet("/tenants",
            async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new GetTenantsQuery(), cancellationToken)))
            .WithSummary("List tenants");

        adminData.MapGet("/core/tenants",
            async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new GetCoreTenantsQuery(), cancellationToken)))
            .WithSummary("List tenants from the core application catalog");

        adminData.MapGet("/subscription-plans",
            async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new GetSubscriptionPlansQuery(), cancellationToken)))
            .WithSummary("List subscription plans");

        adminData.MapGet("/database-instances",
            async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new GetDatabaseInstancesQuery(), cancellationToken)))
            .WithSummary("List database instances");

        adminData.MapPost("/tenants",
            async (UpsertTenantRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                {
                    var tenant = await mediator.Send(new CreateTenantCommand(request), cancellationToken);
                    return Results.Created($"/admin/tenants/{tenant.Identifier}", tenant);
                }))
            .WithSummary("Create tenant");

        adminData.MapPut("/tenants/{identifier}",
            async (string identifier, UpsertTenantRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                    Results.Ok(await mediator.Send(new UpdateTenantCommand(identifier, request), cancellationToken))))
            .WithSummary("Update tenant");

        adminData.MapDelete("/tenants/{identifier}",
            async (string identifier, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                {
                    await mediator.Send(new DeactivateTenantCommand(identifier), cancellationToken);
                    return Results.NoContent();
                }))
            .WithSummary("Deactivate tenant");

        var admin = app.MapGroup("/admin")
            .WithTags("Admin")
            .RequireAuthorization("AdminOnly");

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

        admin.MapPost("/core/tenants/{identifier}/provision",
            async (string identifier, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                    Results.Ok(await mediator.Send(new ProvisionTenantSchemaCommand(identifier), cancellationToken))))
            .WithSummary("Provision or repair a single tenant schema (idempotent — safe to call on already-provisioned tenants)");

        admin.MapPost("/core/tenants/provision/all",
            async ([FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                    Results.Ok(await mediator.Send(new BulkReprovisionTenantsCommand(), cancellationToken))))
            .WithSummary("Re-provision all active tenants — runs EF migrations for any tenant whose schema exists but tables are missing");

        admin.MapPost("/core/migrations/tenants/{identifier}",
            async (string identifier, TenantSchemaMigrationRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                    Results.Ok(await mediator.Send(new RunTenantSchemaMigrationCommand(identifier, request), cancellationToken))))
            .WithSummary("Run a tenant schema migration for one tenant");

        admin.MapPost("/core/migrations/bulk",
            async (TenantSchemaMigrationRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                await ExecuteAdminWriteAsync(async () =>
                    Results.Ok(await mediator.Send(new RunBulkTenantSchemaMigrationCommand(request), cancellationToken))))
            .WithSummary("Run a tenant schema migration across all active tenants");

        var bff = app.MapGroup("/auth/bff/admin").WithTags("Admin BFF");

        var publicApi = app.MapGroup("/public").WithTags("Public");

        publicApi.MapPost("/register",
            async (TenantRegistrationRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new RegisterTenantCommand(request), cancellationToken);
                return result.Succeeded
                    ? Results.Created($"/public/tenants/{result.TenantIdentifier}", result)
                    : Results.BadRequest(new { Result = false, Message = result.Message });
            })
            .AllowAnonymous()
            .WithSummary("Self-register a new tenant and primary admin user");

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

    private static async Task<AdminSessionDto> BuildSessionAsync(HttpContext httpContext, AuthOptions authOptions)
    {
        var cookieResult = await httpContext.AuthenticateAsync(AdminCookieScheme);
        var bearerResult = await httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        var expiresAtUtc = cookieResult.Succeeded
            ? cookieResult.Properties?.ExpiresUtc
            : bearerResult.Properties?.ExpiresUtc ?? TryGetTokenExpiration(httpContext.User);

        return new AdminSessionDto
        {
            IsAuthenticated = httpContext.User.Identity?.IsAuthenticated == true,
            ShellModeEnabled = authOptions.AdminBff.ShellModeEnabled,
            AuthenticationMode = ResolveAuthenticationMode(httpContext, cookieResult.Succeeded),
            ExpiresAtUtc = expiresAtUtc,
            LoginPath = authOptions.AdminBff.LoginPath,
            LogoutPath = authOptions.AdminBff.LogoutPath,
            CsrfHeaderName = authOptions.AdminBff.CsrfHeaderName,
            AccessToken = ResolveAccessToken(httpContext, cookieResult, bearerResult),
            User = BuildSessionUser(httpContext.User)
        };
    }

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

        var objectId = principal.FindFirstValue(AuthClaimTypes.ObjectId)
                       ?? principal.FindFirstValue(AuthClaimTypes.Subject)
                       ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? string.Empty;

        return new AdminSessionUserDto
        {
            UserId = objectId,
            ObjectId = objectId,
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

    private static string? ResolveAccessToken(
        HttpContext httpContext,
        AuthenticateResult cookieResult,
        AuthenticateResult bearerResult)
    {
        var cookieToken = cookieResult.Properties?.GetTokenValue("access_token");
        if (!string.IsNullOrWhiteSpace(cookieToken))
            return cookieToken;

        var bearerToken = bearerResult.Properties?.GetTokenValue("access_token");
        if (!string.IsNullOrWhiteSpace(bearerToken))
            return bearerToken;

        var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();
        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        return authorizationHeader["Bearer ".Length..].Trim();
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

    private static Task<IResult> ExecuteAdminWriteAsync(Func<Task<IResult>> action)
        => ExecuteAdminReadAsync(action, Results.Conflict);

    private static async Task<IResult> ExecuteAdminReadAsync(
        Func<Task<IResult>> action,
        Func<object, IResult>? conflictFactory = null)
    {
        try
        {
            return await action();
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { Result = false, Message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return (conflictFactory ?? Results.Conflict)(new { Result = false, Message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return Results.NotFound(new { Result = false, Message = exception.Message });
        }
    }
}
