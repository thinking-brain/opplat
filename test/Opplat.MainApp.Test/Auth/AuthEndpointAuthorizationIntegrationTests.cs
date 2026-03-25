using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Globalization;
using System.Text.Encodings.Web;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Opplat.MainApp.Auth;
using Opplat.MainApp.Dtos;
using Opplat.MainApp.Features.Account;
using Opplat.MainApp.Features.Account.Commands;
using Opplat.MainApp.Features.Account.Queries;
using Opplat.MainApp.Features.Admin;
using Opplat.MainApp.Features.Admin.Commands;
using Opplat.MainApp.Features.Admin.Queries;
using Opplat.MainApp.Middleware;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Test.Auth;

public class AuthEndpointAuthorizationIntegrationTests
{
    [Theory]
    [InlineData("/admin/users")]
    [InlineData("/auth/account/user-list")]
    [InlineData("/auth/account/profile/self")]
    public async Task ProtectedEndpoints_RejectAnonymousRequests(string path)
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/admin/users")]
    [InlineData("/admin/tenants/mojocafe/users")]
    public async Task SuperAdmin_CanAccessAdminPanelAndTenantUserManagement(string path)
    {
        await using var app = await CreateAppAsync();
        var response = await CreateAuthenticatedClient(app, AuthRoles.SuperAdmin).GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TenantScopedAdminEndpoints_RejectRequestsWithoutAResolvedTenantHeader()
    {
        await using var app = await CreateAppAsync(resolvedTenantIdentifier: null);
        var client = CreateAuthenticatedClient(app, AuthRoles.SuperAdmin);

        var response = await client.GetAsync("/admin/tenants/mojocafe/users");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("X-Tenant-Identifier header must identify the tenant", body);
    }

    [Fact]
    public async Task TenantScopedAdminEndpoints_RejectRequestsWhenResolvedTenantDoesNotMatchTheRoute()
    {
        await using var app = await CreateAppAsync(resolvedTenantIdentifier: "demo");
        var client = CreateAuthenticatedClient(app, AuthRoles.SuperAdmin);

        var response = await client.GetAsync("/admin/tenants/mojocafe/users");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("tenant route segment must match the resolved tenant header", body);
    }

    [Fact]
    public async Task TenantScopedAdminEndpoints_AcceptRequestsWhenTenantHeaderAndClaimsMatchTheResolvedTenant()
    {
        await using var app = await CreateAppAsync();
        var client = CreateAuthenticatedClient(
            app,
            [AuthRoles.SuperAdmin],
            [
                new Claim(AuthClaimTypes.TenantId, "tenant-a"),
                new Claim(AuthClaimTypes.TenantIdentifier, "mojocafe")
            ]);

        var response = await client.GetAsync("/admin/tenants/mojocafe/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(AuthClaimTypes.TenantId, "tenant-b")]
    [InlineData(AuthClaimTypes.TenantIdentifier, "demo")]
    public async Task TenantScopedAdminEndpoints_RejectRequestsWhenTenantClaimsDoNotMatchTheResolvedTenant(
        string claimType,
        string claimValue)
    {
        await using var app = await CreateAppAsync();
        var client = CreateAuthenticatedClient(
            app,
            [AuthRoles.SuperAdmin],
            [new Claim(claimType, claimValue)]);

        var response = await client.GetAsync("/admin/tenants/mojocafe/users");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("Token not valid for this tenant.", body);
    }

    [Fact]
    public async Task TenantAdmin_CanManageTenantUsersButCannotAccessAdminPanel()
    {
        await using var app = await CreateAppAsync();
        var client = CreateAuthenticatedClient(app, AuthRoles.TenantAdmin, AuthRoles.TenantUser);

        var listResponse = await client.GetAsync("/auth/account/user-list");
        var createResponse = await client.PostAsJsonAsync("/auth/account/add-user", new Register
        {
            Name = "Tenant",
            LastName = "Manager",
            Username = "tenant.manager",
            Email = "tenant.manager@opplat.local",
            Password = "ignored-by-idp",
            ConfirmPassword = "ignored-by-idp"
        });
        var adminResponse = await client.GetAsync("/admin/users");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, adminResponse.StatusCode);
    }

    [Fact]
    public async Task TenantUser_HasReadOnlyAccessAndIsRejectedFromAdminOperations()
    {
        await using var app = await CreateAppAsync();
        var client = CreateAuthenticatedClient(app, AuthRoles.TenantUser);

        var profileResponse = await client.GetAsync("/auth/account/profile/self");
        var userListResponse = await client.GetAsync("/auth/account/user-list");
        var addUserResponse = await client.PostAsJsonAsync("/auth/account/add-user", new Register
        {
            Name = "Tenant",
            LastName = "Viewer",
            Username = "tenant.viewer",
            Email = "tenant.viewer@opplat.local",
            Password = "ignored-by-idp",
            ConfirmPassword = "ignored-by-idp"
        });
        var adminResponse = await client.GetAsync("/admin/users");

        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, userListResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, addUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, adminResponse.StatusCode);
    }

    private static HttpClient CreateAuthenticatedClient(WebApplication app, params string[] roles) =>
        CreateAuthenticatedClient(app, roles, claims: null, expiresAtUtc: null);

    private static HttpClient CreateAuthenticatedClient(
        WebApplication app,
        string[]? roles = null,
        Claim[]? claims = null,
        DateTimeOffset? expiresAtUtc = null)
    {
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.AuthenticatedHeader, "true");
        if (roles is { Length: > 0 })
            client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, string.Join(',', roles));

        if (claims is { Length: > 0 })
        {
            client.DefaultRequestHeaders.Add(
                TestAuthHandler.ClaimsHeader,
                claims.Select(claim => $"{claim.Type}={claim.Value}"));
        }

        if (expiresAtUtc is not null)
        {
            client.DefaultRequestHeaders.Add(
                TestAuthHandler.ExpiresAtUtcHeader,
                expiresAtUtc.Value.ToString("O", CultureInfo.InvariantCulture));
        }

        return client;
    }

    private static async Task<WebApplication> CreateAppAsync(
        string? resolvedTenantId = "tenant-a",
        string? resolvedTenantIdentifier = "mojocafe",
        bool shellModeEnabled = false)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.WebHost.UseTestServer();
        builder.Services.AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        builder.Services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, TestAdminCookieHandler>("AdminCookie", _ => { });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.SuperAdmin));
            options.AddPolicy("TenantAdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.TenantAdmin));
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
            options.AdminBff.ClientId = "opplat-admin";
            options.AdminBff.DefaultOrigin = "http://localhost:3201";
            options.AdminBff.ShellModeEnabled = shellModeEnabled;
            options.AdminBff.AllowedOrigins =
            [
                "http://localhost:3001",
                "http://localhost:3101",
                "http://localhost:3201",
                "http://localhost:5174"
            ];
        });
        builder.Services.AddSingleton<IMediator>(CreateMediatorMock().Object);
        builder.Services.AddSingleton<IMultiTenantContextAccessor<AppTenantInfo>>(new StaticTenantAccessor(resolvedTenantId, resolvedTenantIdentifier));

        var app = builder.Build();
        app.UseAuthentication();
        app.UseMiddleware<TenantValidationMiddleware>();
        app.UseAuthorization();
        app.MapAccountEndpoints();
        app.MapAdminEndpoints();

        await app.StartAsync();
        return app;
    }

    private static Mock<IMediator> CreateMediatorMock()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountDto>());
        mediator.Setup(x => x.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetUserProfileQuery query, CancellationToken _) => new AccountDto
            {
                UserId = "user-1",
                Username = query.Username,
                Name = "Tenant",
                LastName = "User",
                Email = $"{query.Username}@opplat.local",
                Active = true,
                Roles = [AuthRoles.TenantUser]
            });
        mediator.Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisterUserCommand command, CancellationToken _) => new RegisterUserResult(
                true,
                new AccountDto
                {
                    UserId = "created-user",
                    Username = command.Username,
                    Name = command.Name,
                    LastName = command.LastName,
                    Email = command.Email,
                    Active = true,
                    Roles = [AuthRoles.TenantUser]
                },
                null));
        mediator.Setup(x => x.Send(It.IsAny<EditUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mediator.Setup(x => x.Send(It.IsAny<ToggleUserActiveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mediator.Setup(x => x.Send(It.IsAny<ChangeRolesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeRolesResult(true, null));
        mediator.Setup(x => x.Send(It.IsAny<GetTenantsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminTenantDto>());
        mediator.Setup(x => x.Send(It.IsAny<GetAdminUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminUserDto>());
        mediator.Setup(x => x.Send(It.IsAny<GetTenantUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminUserDto>());
        mediator.Setup(x => x.Send(It.IsAny<CreateTenantUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTenantUserCommand command, CancellationToken _) => new AdminUserDto
            {
                UserId = "tenant-user-1",
                Username = command.Username,
                Name = command.Name,
                LastName = command.LastName,
                Email = command.Email,
                Active = true,
                Roles = command.Roles.ToList(),
                TenantId = "tenant-a",
                TenantIdentifier = "mojocafe",
                TenantName = "MojoCafe"
            });
        mediator.Setup(x => x.Send(It.IsAny<UpdateTenantUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mediator.Setup(x => x.Send(It.IsAny<SetUserActiveStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return mediator;
    }

    private sealed class StaticTenantAccessor : IMultiTenantContextAccessor<AppTenantInfo>
    {
        private readonly IMultiTenantContext<AppTenantInfo> _multiTenantContext;

        public StaticTenantAccessor(string? tenantId, string? tenantIdentifier)
        {
            var tenantContext = new Mock<IMultiTenantContext<AppTenantInfo>>();
            tenantContext.SetupGet(context => context.TenantInfo).Returns(new AppTenantInfo
            {
                Id = tenantId,
                Identifier = tenantIdentifier,
                Name = tenantIdentifier ?? tenantId ?? string.Empty
            });

            _multiTenantContext = tenantContext.Object;
        }

        public IMultiTenantContext<AppTenantInfo> MultiTenantContext => _multiTenantContext;

        IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => _multiTenantContext;
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestAuth";
        public const string AdminCookieScheme = "AdminCookie";
        public const string AuthenticatedHeader = "X-Test-Authenticated";
        public const string RolesHeader = "X-Test-Roles";
        public const string ClaimsHeader = "X-Test-Claim";
        public const string ExpiresAtUtcHeader = "X-Test-ExpiresUtc";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(CreateAuthenticateResult(Request, Scheme.Name));
        }

        internal static AuthenticateResult CreateAuthenticateResult(HttpRequest request, string schemeName)
        {
            if (!request.Headers.TryGetValue(AuthenticatedHeader, out var isAuthenticated) ||
                !string.Equals(isAuthenticated.ToString(), "true", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

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
                claims.Add(new Claim(ClaimTypes.Name, "test-user"));

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

            var ticket = new AuthenticationTicket(principal, properties, schemeName);
            return AuthenticateResult.Success(ticket);
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

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
            Task.FromResult(TestAuthHandler.CreateAuthenticateResult(Request, Scheme.Name));

        protected override Task HandleSignOutAsync(AuthenticationProperties? properties)
        {
            Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        }
    }

}
