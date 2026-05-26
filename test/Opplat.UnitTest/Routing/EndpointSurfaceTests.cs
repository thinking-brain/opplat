using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MediatR;
using Finbuckle.MultiTenant.Abstractions;
using Opplat.Domain.Models;

namespace Opplat.UnitTest.Routing;

public class EndpointSurfaceTests
{
    [Fact]
    public void MapAccountEndpoints_ExposesTenantAwareAndLegacyLoginRoutes()
    {
        using var app = CreateApp();

        // app.MapAccountEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/{__tenant__}/auth/account/Login", patterns);
        Assert.Contains("/auth/account/Login", patterns);
    }

    [Fact]
    public void MapAccountEndpoints_MarksLoginAnonymousAndUserListAuthorized()
    {
        using var app = CreateApp();

        // app.MapAccountEndpoints();

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

        // app.MapAdminEndpoints();
        // app.MapMenusEndpoints();
        // app.MapLicenseEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/admin/tenants", patterns);
        Assert.Contains("/admin/users", patterns);
        Assert.Contains("/admin/menus", patterns);
        Assert.Contains("/admin/menus/FromModulo", patterns);
    }

    [Fact]
    public void MapAdminEndpoints_RequireAdminOnlyAuthorizationPolicy()
    {
        using var app = CreateApp();

        // app.MapAdminEndpoints();

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

    [Fact]
    public void MapInventoryEndpoints_ExposeRootAndTenantAwareRouteSurface()
    {
        using var app = CreateApp();

        // app.MapInventoryEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/inventory/products", patterns);
        Assert.Contains("/inventory/products/{id}", patterns);
        Assert.Contains("/inventory/storages", patterns);
        Assert.Contains("/inventory/inventories/{id}", patterns);
        Assert.Contains("/inventory/productmovements", patterns);
        Assert.Contains("/inventory/movementtypes", patterns);
        Assert.Contains("/{__tenant__}/inventory/products", patterns);
        Assert.Contains("/{__tenant__}/inventory/products/{id}", patterns);
        Assert.Contains("/{__tenant__}/inventory/storages", patterns);
        Assert.Contains("/{__tenant__}/inventory/inventories/{id}", patterns);
        Assert.Contains("/{__tenant__}/inventory/productmovements", patterns);
        Assert.Contains("/{__tenant__}/inventory/movementtypes", patterns);
    }

    [Fact]
    public void MapInventoryEndpoints_KeepMovementTypesProtectedWhileOtherInventoryReadsStayUnannotated()
    {
        using var app = CreateApp();

        // app.MapInventoryEndpoints();

        var endpoints = GetRouteEndpoints(app).ToList();
        var movementTypeEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/inventory/movementtypes", StringComparison.Ordinal))
            .ToList();
        var productsEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/inventory/products", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(movementTypeEndpoints);
        Assert.NotEmpty(productsEndpoints);
        Assert.All(movementTypeEndpoints, endpoint => Assert.Contains(endpoint.Metadata, metadata => metadata is IAuthorizeData));
        Assert.All(productsEndpoints, endpoint => Assert.DoesNotContain(endpoint.Metadata, metadata => metadata is IAuthorizeData));
    }

    [Fact]
    public void MapSalesEndpoints_ExposeRootAndTenantAwareRouteSurface()
    {
        using var app = CreateApp();

        // app.MapSalesEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/sales", patterns);
        Assert.Contains("/sales/products", patterns);
        Assert.Contains("/sales/products/{id}", patterns);
        Assert.Contains("/sales/toppings", patterns);
        Assert.Contains("/sales/toppings/{id}", patterns);
        Assert.Contains("/sales/producttags", patterns);
        Assert.Contains("/sales/producttags/{id}", patterns);
        Assert.Contains("/sales/costtabs", patterns);
        Assert.Contains("/sales/costtabs/{id}", patterns);
        Assert.Contains("/{__tenant__}/sales", patterns);
        Assert.Contains("/{__tenant__}/sales/products", patterns);
        Assert.Contains("/{__tenant__}/sales/products/{id}", patterns);
        Assert.Contains("/{__tenant__}/sales/toppings", patterns);
        Assert.Contains("/{__tenant__}/sales/toppings/{id}", patterns);
        Assert.Contains("/{__tenant__}/sales/producttags", patterns);
        Assert.Contains("/{__tenant__}/sales/producttags/{id}", patterns);
        Assert.Contains("/{__tenant__}/sales/costtabs", patterns);
        Assert.Contains("/{__tenant__}/sales/costtabs/{id}", patterns);
    }

    [Fact]
    public void MapSalesEndpoints_KeepSalesListProtectedWhileOtherSalesReadsStayUnannotated()
    {
        using var app = CreateApp();

        // app.MapSalesEndpoints();

        var endpoints = GetRouteEndpoints(app).ToList();
        var salesListEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/sales", StringComparison.Ordinal))
            .ToList();
        var productsEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                Normalize(endpoint.RoutePattern.RawText).EndsWith("/sales/products", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(salesListEndpoints);
        Assert.NotEmpty(productsEndpoints);
        Assert.All(salesListEndpoints, endpoint => Assert.Contains(endpoint.Metadata, metadata => metadata is IAuthorizeData));
        Assert.All(productsEndpoints, endpoint => Assert.DoesNotContain(endpoint.Metadata, metadata => metadata is IAuthorizeData));
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
