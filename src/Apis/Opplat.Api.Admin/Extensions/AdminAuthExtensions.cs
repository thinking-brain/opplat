using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Opplat.Api.Admin.Auth;
using Opplat.Api.Admin.Hosting;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;

namespace Opplat.Api.Admin.Extensions;

public static class AdminAuthExtensions
{
    public static IServiceCollection AddAdminAuthentication(
        this IServiceCollection services,
        AuthOptions authOptions,
        AuthRuntimeConfiguration authRuntime,
        AdminBffOptions adminBffOptions,
        bool requireHttpsMetadata)
    {
        services.AddTransient<IClaimsTransformation, OidcClaimsTransformation>();
        services.AddMemoryCache();
        services.AddSingleton<MemoryCacheTicketStore>();
        services.AddAntiforgery(options =>
        {
            options.Cookie.Name = adminBffOptions.CsrfCookieName;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = requireHttpsMetadata ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            options.HeaderName = adminBffOptions.CsrfHeaderName;
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "AdminApiAuth";
                options.DefaultChallengeScheme = "AdminApiAuth";
                options.DefaultForbidScheme = "AdminApiAuth";
                options.DefaultScheme = "AdminApiAuth";
            })
            .AddPolicyScheme("AdminApiAuth", "Cookie or bearer", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return JwtBearerDefaults.AuthenticationScheme;

                    return context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase)
                           || context.Request.Path.StartsWithSegments("/auth/bff/admin", StringComparison.OrdinalIgnoreCase)
                           || context.Request.Path.StartsWithSegments(adminBffOptions.CallbackPath, StringComparison.OrdinalIgnoreCase)
                           || context.Request.Path.StartsWithSegments(adminBffOptions.SignedOutCallbackPath, StringComparison.OrdinalIgnoreCase)
                        ? "AdminCookie"
                        : JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.MapInboundClaims = false;
                options.Authority = authRuntime.Authority;
                if (!string.IsNullOrWhiteSpace(authRuntime.MetadataAddress))
                    options.MetadataAddress = authRuntime.MetadataAddress;
                options.Audience = authRuntime.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidIssuers = authRuntime.ValidIssuers,
                    NameClaimType = AuthClaimTypes.PreferredUserName,
                    RoleClaimType = ClaimTypes.Role
                };
            })
            .AddCookie("AdminCookie", options =>
            {
                options.Cookie.Name = adminBffOptions.SessionCookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = requireHttpsMetadata ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = TimeSpan.FromHours(Math.Max(1, adminBffOptions.SessionHours));
                options.SlidingExpiration = true;
                options.LoginPath = adminBffOptions.LoginPath;
                options.AccessDeniedPath = adminBffOptions.AccessDeniedPath;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddOpenIdConnect("AdminOidc", options =>
            {
                options.SignInScheme = "AdminCookie";
                options.MapInboundClaims = false;
                options.Authority = authRuntime.Authority;
                options.MetadataAddress = string.IsNullOrWhiteSpace(authRuntime.MetadataAddress)
                    ? null
                    : authRuntime.MetadataAddress;
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.ClientId = adminBffOptions.ClientId;
                options.ClientSecret = string.IsNullOrWhiteSpace(adminBffOptions.ClientSecret)
                    ? null
                    : adminBffOptions.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = adminBffOptions.UsePkce;
                options.GetClaimsFromUserInfoEndpoint = adminBffOptions.GetClaimsFromUserInfoEndpoint;
                options.CallbackPath = adminBffOptions.CallbackPath;
                options.SignedOutCallbackPath = adminBffOptions.SignedOutCallbackPath;
                options.SaveTokens = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = authRuntime.ValidIssuers,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
                options.Scope.Clear();
                foreach (var scope in adminBffOptions.Scopes.Where(scope => !string.IsNullOrWhiteSpace(scope)).Distinct(StringComparer.OrdinalIgnoreCase))
                    options.Scope.Add(scope);

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                            OidcClaimsNormalizer.Normalize(identity, authOptions);

                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        if (adminBffOptions.UseAudienceQueryParam && !string.IsNullOrWhiteSpace(authOptions.Audience))
                            context.ProtocolMessage.SetParameter("audience", authOptions.Audience);

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddOptions<CookieAuthenticationOptions>("AdminCookie")
            .Configure<MemoryCacheTicketStore>((options, ticketStore) =>
            {
                options.SessionStore = ticketStore;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole(authOptions.AdminRole));
        });

        return services;
    }
}
