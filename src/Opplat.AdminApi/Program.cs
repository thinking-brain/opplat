using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Opplat.AdminApi.Auth;
using Opplat.AdminApi.Data;
using Opplat.AdminApi.Endpoints;
using Opplat.AdminApi.Hosting;
using Opplat.AdminApi.Middleware;
using Opplat.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var authSection = builder.Configuration.GetSection(AuthOptions.SectionName);
var authOptions = authSection.Get<AuthOptions>() ?? new AuthOptions();
var authRuntime = AuthRuntimeConfigurationResolver.Resolve(authOptions);
var adminBffOptions = authOptions.AdminBff;

var legacyAdminClientId = authSection["ClientIdAdmin"];
if (!string.IsNullOrWhiteSpace(legacyAdminClientId))
    adminBffOptions.ClientId = legacyAdminClientId.Trim();

var legacyAdminClientSecret = authSection["ClientSecretAdmin"];
if (!string.IsNullOrWhiteSpace(legacyAdminClientSecret))
    adminBffOptions.ClientSecret = legacyAdminClientSecret.Trim();

var allowedCorsOrigins = adminBffOptions.AllowedOrigins
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();
var requireHttpsMetadata = !builder.Environment.IsDevelopment();

builder.Services.Configure<AuthOptions>(options =>
{
    options.Provider = authOptions.Provider;
    options.Authority = authRuntime.Authority;
    options.MetadataAddress = authRuntime.MetadataAddress;
    options.Audience = authRuntime.Audience;
    options.ValidIssuers = [.. authRuntime.ValidIssuers];
    options.ClaimNamespace = authOptions.ClaimNamespace;
    options.AdminRole = authOptions.AdminRole;
    options.TenantAdminRole = authOptions.TenantAdminRole;
    options.TenantUserRole = authOptions.TenantUserRole;
    options.AdditionalRoleClaimTypes = [.. authOptions.AdditionalRoleClaimTypes];
    options.Entra = new EntraAuthOptions
    {
        TenantId = authOptions.Entra.TenantId,
        AuthorityHost = authOptions.Entra.AuthorityHost,
        UseV2Endpoint = authOptions.Entra.UseV2Endpoint,
        ObjectIdClaimType = authOptions.Entra.ObjectIdClaimType
    };
    options.AdminBff = new AdminBffOptions
    {
        ClientId = adminBffOptions.ClientId,
        ClientSecret = adminBffOptions.ClientSecret,
        ShellModeEnabled = adminBffOptions.ShellModeEnabled,
        Scopes = [.. adminBffOptions.Scopes],
        UsePkce = adminBffOptions.UsePkce,
        GetClaimsFromUserInfoEndpoint = adminBffOptions.GetClaimsFromUserInfoEndpoint,
        UseAudienceQueryParam = adminBffOptions.UseAudienceQueryParam,
        LoginPath = adminBffOptions.LoginPath,
        LogoutPath = adminBffOptions.LogoutPath,
        CallbackPath = adminBffOptions.CallbackPath,
        SignedOutCallbackPath = adminBffOptions.SignedOutCallbackPath,
        AccessDeniedPath = adminBffOptions.AccessDeniedPath,
        SessionCookieName = adminBffOptions.SessionCookieName,
        CsrfCookieName = adminBffOptions.CsrfCookieName,
        CsrfHeaderName = adminBffOptions.CsrfHeaderName,
        SessionHours = adminBffOptions.SessionHours,
        DefaultOrigin = adminBffOptions.DefaultOrigin,
        AllowedOrigins = [.. adminBffOptions.AllowedOrigins]
    };
});
builder.Services.AddDbContext<AdminTenantCatalogDbContext>(options =>
    options.UseNpgsql(NormalizePostgresConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, OidcClaimsTransformation>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<MemoryCacheTicketStore>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = adminBffOptions.CsrfCookieName;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = requireHttpsMetadata ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
    options.HeaderName = adminBffOptions.CsrfHeaderName;
});

builder.Services.AddAuthentication(options =>
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

builder.Services.AddOptions<CookieAuthenticationOptions>("AdminCookie")
    .Configure<MemoryCacheTicketStore>((options, ticketStore) =>
    {
        options.SessionStore = ticketStore;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(_ => true));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        if (allowedCorsOrigins.Length == 0)
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            return;
        }

        policy.WithOrigins(allowedCorsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);
builder.Services.AddGraphUserService(builder.Configuration);

var app = builder.Build();
await AdminPortalDataSeeder.InitializeAsync(app.Services);

app.UseOpplatAspireDevelopmentSupport();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseWhen(
        context => !IsAdminBffDevelopmentRequest(context.Request.Path, adminBffOptions),
        branch => branch.UseHttpsRedirectionIfConfigured(app.Configuration));
}
else
{
    app.UseHttpsRedirectionIfConfigured();
}

app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AdminBffAntiforgeryMiddleware>();
app.UseAuthorization();

app.MapGeneralEndpoints();
app.MapAdminEndpoints();
app.Run();

static bool IsAdminBffDevelopmentRequest(PathString path, AdminBffOptions adminBffOptions) =>
    path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase)
    || path.StartsWithSegments("/auth/bff/admin", StringComparison.OrdinalIgnoreCase)
    || path.StartsWithSegments(adminBffOptions.CallbackPath, StringComparison.OrdinalIgnoreCase)
    || path.StartsWithSegments(adminBffOptions.SignedOutCallbackPath, StringComparison.OrdinalIgnoreCase);

static string NormalizePostgresConnectionString(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");

    var builder = new NpgsqlConnectionStringBuilder(connectionString);
    if (builder.SslMode == SslMode.Prefer)
        builder.SslMode = SslMode.Disable;

    return builder.ConnectionString;
}

