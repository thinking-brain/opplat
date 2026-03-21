using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

    private static HttpClient CreateAuthenticatedClient(WebApplication app, params string[] roles)
    {
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.AuthenticatedHeader, "true");
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, string.Join(',', roles));
        return client;
    }

    private static async Task<WebApplication> CreateAppAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.WebHost.UseTestServer();
        builder.Services.AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.SuperAdmin));
            options.AddPolicy("TenantAdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.TenantAdmin));
        });
        builder.Services.AddSingleton<IMediator>(CreateMediatorMock().Object);
        builder.Services.AddSingleton<IMultiTenantContextAccessor<AppTenantInfo>>(new StaticTenantAccessor("tenant-a", "mojocafe"));

        var app = builder.Build();
        app.UseAuthentication();
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

        public StaticTenantAccessor(string tenantId, string tenantIdentifier)
        {
            var tenantContext = new Mock<IMultiTenantContext<AppTenantInfo>>();
            tenantContext.SetupGet(context => context.TenantInfo).Returns(new AppTenantInfo
            {
                Id = tenantId,
                Identifier = tenantIdentifier,
                Name = tenantIdentifier
            });

            _multiTenantContext = tenantContext.Object;
        }

        public IMultiTenantContext<AppTenantInfo> MultiTenantContext => _multiTenantContext;

        IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => _multiTenantContext;
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestAuth";
        public const string AuthenticatedHeader = "X-Test-Authenticated";
        public const string RolesHeader = "X-Test-Roles";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(AuthenticatedHeader, out var isAuthenticated) ||
                !string.Equals(isAuthenticated.ToString(), "true", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "test-user")
            };

            if (Request.Headers.TryGetValue(RolesHeader, out var rolesHeader))
            {
                claims.AddRange(
                    rolesHeader.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
