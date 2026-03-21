using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MediatR;
using Opplat.MainApp.Features.Admin;
using Opplat.MainApp.Features.Account;
using Opplat.MainApp.Features.License;
using Opplat.MainApp.Features.Menus;
using Finbuckle.MultiTenant.Abstractions;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Test.Routing;

public class EndpointSurfaceTests
{
    [Fact]
    public void MapAccountEndpoints_ExposesTenantAwareAndLegacyLoginRoutes()
    {
        using var app = CreateApp();

        app.MapAccountEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/{__tenant__}/auth/account/Login", patterns);
        Assert.Contains("/auth/account/Login", patterns);
    }

    [Fact]
    public void MapAccountEndpoints_MarksLoginAnonymousAndUserListAuthorized()
    {
        using var app = CreateApp();

        app.MapAccountEndpoints();

        var endpoints = GetRouteEndpoints(app).ToList();
        var loginEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/auth/account/Login"))
            .ToList();
        var userListEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/auth/account/user-list"))
            .ToList();

        Assert.NotEmpty(loginEndpoints);
        Assert.NotEmpty(userListEndpoints);
        Assert.All(loginEndpoints, endpoint => Assert.Contains(endpoint.Metadata, metadata => metadata is IAllowAnonymous));
        Assert.All(userListEndpoints, endpoint => Assert.Contains(endpoint.Metadata, metadata => metadata is IAuthorizeData));
    }

    [Fact]
    public void AdminEndpointGroups_StayUnderAdminPrefix()
    {
        using var app = CreateApp();

        app.MapAdminEndpoints();
        app.MapMenusEndpoints();
        app.MapLicenseEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/admin/tenants", patterns);
        Assert.Contains("/admin/users", patterns);
        Assert.Contains("/admin/menus", patterns);
        Assert.Contains("/admin/menus/FromModulo", patterns);
        Assert.Contains("/admin/licencia", patterns);
    }

    [Fact]
    public void MapAdminEndpoints_RequireAdminOnlyAuthorizationPolicy()
    {
        using var app = CreateApp();

        app.MapAdminEndpoints();

        var adminEndpoints = GetRouteEndpoints(app)
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).StartsWith("/admin", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(adminEndpoints);
        Assert.All(adminEndpoints, endpoint =>
        {
            var authorizeData = endpoint.Metadata.OfType<IAuthorizeData>().ToList();

            Assert.NotEmpty(authorizeData);
            Assert.Contains(authorizeData, data => string.Equals(data.Policy, "AdminOnly", StringComparison.Ordinal));
        });
    }

    private static WebApplication CreateApp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();
        builder.Services.AddSingleton(Mock.Of<IMediator>());
        builder.Services.AddSingleton(Mock.Of<IMultiTenantContextAccessor<AppTenantInfo>>());

        return builder.Build();
    }

    private static IReadOnlyList<RouteEndpoint> GetRouteEndpoints(WebApplication app)
    {
        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();
    }

    private static IReadOnlyList<string> GetRoutePatterns(WebApplication app)
    {
        return GetRouteEndpoints(app)
            .Where(endpoint => endpoint.RoutePattern.RawText is not null)
            .Select(endpoint => Normalize(endpoint.RoutePattern.RawText!))
            .ToList();
    }

    private static string Normalize(string routePattern)
    {
        return routePattern.Length > 1 ? routePattern.TrimEnd('/') : routePattern;
    }
}
