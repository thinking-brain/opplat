using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.AdminApi.Endpoints;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services;
using Opplat.Application.Abstractions.Services;

namespace Opplat.UnitTest.Auth;

public class AdminApiSessionEndpointTests
{
    [Theory]
    [InlineData("/admin/session")]
    [InlineData("/admin/session/current-user")]
    public async Task SessionEndpoints_BearerAuthExposeStableOid_Expiry_AndAccessToken(string path)
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();
        var exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "opaque-access-token");
        client.DefaultRequestHeaders.Add(TestSessionHandler.BearerAuthenticatedHeader, "true");
        client.DefaultRequestHeaders.Add(TestSessionHandler.RolesHeader, AuthRoles.SuperAdmin);
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "oid=entra-oid-123");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "preferred_username=admin.user");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "given_name=Admin");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "family_name=User");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "email=admin.user@opplat.local");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, $"exp={exp}");

        var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        var user = root.GetProperty("user");

        Assert.True(root.GetProperty("isAuthenticated").GetBoolean());
        Assert.Equal("bearer", root.GetProperty("authenticationMode").GetString());
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(exp), root.GetProperty("expiresAtUtc").GetDateTimeOffset());
        Assert.Equal("opaque-access-token", root.GetProperty("accessToken").GetString());

        Assert.Equal("entra-oid-123", user.GetProperty("userId").GetString());
        Assert.Equal("entra-oid-123", user.GetProperty("objectId").GetString());
        Assert.Equal("admin.user", user.GetProperty("username").GetString());
        Assert.Equal("Admin", user.GetProperty("name").GetString());
        Assert.Equal("User", user.GetProperty("lastName").GetString());
        Assert.Equal("admin.user@opplat.local", user.GetProperty("email").GetString());
        Assert.Equal(new[] { AuthRoles.SuperAdmin }, user.GetProperty("roles").EnumerateArray().Select(role => role.GetString()).ToArray());
    }

    [Fact]
    public async Task SessionEndpoint_CookieAuthUsesCookieExpiry_AndKeepsAccessTokenAbsent()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();
        var expiresAtUtc = DateTimeOffset.UtcNow.AddHours(2);

        client.DefaultRequestHeaders.Add(TestSessionHandler.CookieAuthenticatedHeader, "true");
        client.DefaultRequestHeaders.Add(TestSessionHandler.RolesHeader, AuthRoles.SuperAdmin);
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "sub=entra-oid-cookie");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "preferred_username=admin.cookie");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ClaimsHeader, "email=admin.cookie@opplat.local");
        client.DefaultRequestHeaders.Add(TestSessionHandler.ExpiresAtUtcHeader, expiresAtUtc.ToString("O", CultureInfo.InvariantCulture));

        var response = await client.GetAsync("/admin/session");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal("cookie", root.GetProperty("authenticationMode").GetString());
        Assert.Equal(expiresAtUtc.ToUnixTimeSeconds(), root.GetProperty("expiresAtUtc").GetDateTimeOffset().ToUnixTimeSeconds());
        Assert.True(!root.TryGetProperty("accessToken", out var accessToken) || accessToken.ValueKind is JsonValueKind.Null);
        Assert.Equal("entra-oid-cookie", root.GetProperty("user").GetProperty("userId").GetString());
        Assert.Equal("entra-oid-cookie", root.GetProperty("user").GetProperty("objectId").GetString());
    }

    [Fact]
    public void AdminSessionContracts_ExposeStableOidAndOptionalAccessTokenSurface()
    {
        var contractsSource = TestRepository.ReadAllText("src", "Opplat.Application.Abstractions", "Admin", "AdminSessionContracts.cs");
        var programSource = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Program.cs");

        Assert.Contains("public string UserId", contractsSource);
        Assert.Contains("public string ObjectId", contractsSource);
        Assert.Contains("public string? AccessToken", contractsSource);
        Assert.Contains("options.SaveTokens = true;", programSource);
    }

    private static async Task<WebApplication> CreateStartedAppAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.WebHost.UseTestServer();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestSessionHandler.SchemeName;
                options.DefaultChallengeScheme = TestSessionHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestSessionHandler>(TestSessionHandler.SchemeName, _ => { })
            .AddScheme<AuthenticationSchemeOptions, TestSessionHandler>(JwtBearerDefaults.AuthenticationScheme, _ => { })
            .AddScheme<AuthenticationSchemeOptions, TestAdminCookieHandler>("AdminCookie", _ => { });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.SuperAdmin));
        });
        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.Name = "opplat.admin.csrf";
            options.HeaderName = "X-Opplat-CSRF";
        });
        builder.Services.Configure<AuthOptions>(options =>
        {
            options.AdminRole = AuthRoles.SuperAdmin;
            options.TenantAdminRole = AuthRoles.TenantAdmin;
            options.TenantUserRole = AuthRoles.TenantUser;
            options.AdminBff.ShellModeEnabled = false;
        });
        builder.Services.AddDbContext<AdminTenantCatalogDbContext>(options =>
            options.UseInMemoryDatabase($"admin-session-tests-{Guid.NewGuid():N}"));
        builder.Services.Configure<DatabaseInstanceOptions>(options =>
        {
            options.DefaultConnectionString = "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*";
        });
        builder.Services.AddSingleton(new DatabaseInstanceOptions
        {
            DefaultConnectionString = "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*"
        });
        builder.Services.AddScoped<TenantSchemaProvisioningService>();
        builder.Services.AddScoped<DatabaseInstanceAutoScalingService>();
        builder.Services.AddScoped<ITenantSchemaMigrationRunner, TenantSchemaMigrationRunner>();
        builder.Services.AddScoped<ITenantProvisioningCoordinator, TenantProvisioningCoordinator>();
        builder.Services.AddSingleton<IEnumerable<Opplat.Infrastructure.Services.ITenantProvisioningReporter>>([]);
        builder.Services.AddSingleton<IEnumerable<Opplat.Application.Abstractions.Services.ITenantSchemaMigrationReporter>>([]);
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Opplat.AdminApi.Endpoints.AdminEndpoints).Assembly));

        var app = builder.Build();
        // await AdminPortalDataSeeder.InitializeAsync(app.Services);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapAdminEndpoints();
        await app.StartAsync();
        return app;
    }

    private sealed class TestSessionHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestSession";
        public const string BearerAuthenticatedHeader = "X-Test-Bearer-Authenticated";
        public const string CookieAuthenticatedHeader = "X-Test-Cookie-Authenticated";
        public const string RolesHeader = "X-Test-Roles";
        public const string ClaimsHeader = "X-Test-Claim";
        public const string ExpiresAtUtcHeader = "X-Test-ExpiresUtc";

        public TestSessionHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var isBearer = Request.Headers.TryGetValue(BearerAuthenticatedHeader, out var bearerAuthenticated)
                           && string.Equals(bearerAuthenticated.ToString(), "true", StringComparison.OrdinalIgnoreCase);
            var isCookie = Scheme.Name == SchemeName
                           && Request.Headers.TryGetValue(CookieAuthenticatedHeader, out var cookieAuthenticated)
                           && string.Equals(cookieAuthenticated.ToString(), "true", StringComparison.OrdinalIgnoreCase);

            if (!isBearer && !isCookie)
                return Task.FromResult(AuthenticateResult.NoResult());

            return Task.FromResult(CreateAuthenticateResult(Request, Scheme.Name));
        }

        internal static AuthenticateResult CreateAuthenticateResult(HttpRequest request, string schemeName)
        {
            var claims = new List<Claim>();

            if (request.Headers.TryGetValue(RolesHeader, out var rolesHeader))
            {
                claims.AddRange(
                    rolesHeader.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(role => new Claim(ClaimTypes.Role, role)));
            }

            if (request.Headers.TryGetValue(ClaimsHeader, out var claimHeaders))
            {
                claims.AddRange(
                    claimHeaders
                        .Select(ParseClaim)
                        .Where(claim => claim is not null)
                        .Cast<Claim>());
            }

            if (!claims.Any(claim => string.Equals(claim.Type, ClaimTypes.Name, StringComparison.Ordinal)))
                claims.Add(new Claim(ClaimTypes.Name, "test-admin"));

            var identity = new ClaimsIdentity(claims, schemeName);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties();
            if (request.Headers.TryGetValue(ExpiresAtUtcHeader, out var expiresHeader) &&
                DateTimeOffset.TryParse(
                    expiresHeader.ToString(),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var expiresAtUtc))
            {
                properties.ExpiresUtc = expiresAtUtc;
            }

            return AuthenticateResult.Success(new AuthenticationTicket(principal, properties, schemeName));
        }

        private static Claim? ParseClaim(string? rawClaim)
        {
            if (string.IsNullOrWhiteSpace(rawClaim))
                return null;

            var separatorIndex = rawClaim.IndexOf('=');
            if (separatorIndex <= 0 || separatorIndex == rawClaim.Length - 1)
                return null;

            var type = rawClaim[..separatorIndex].Trim();
            var value = rawClaim[(separatorIndex + 1)..].Trim();
            return string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value)
                ? null
                : new Claim(type, value);
        }
    }

    private sealed class TestAdminCookieHandler : SignOutAuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAdminCookieHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(TestSessionHandler.CookieAuthenticatedHeader, out var cookieAuthenticated) ||
                !string.Equals(cookieAuthenticated.ToString(), "true", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            return Task.FromResult(TestSessionHandler.CreateAuthenticateResult(Request, Scheme.Name));
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties? properties)
        {
            Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        }
    }
}
