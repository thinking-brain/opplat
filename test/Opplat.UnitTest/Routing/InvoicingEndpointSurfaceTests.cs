using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Finbuckle.MultiTenant.Abstractions;
using Opplat.Api.Invoicing.Endpoints;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Domain.Models;

namespace Opplat.UnitTest.Routing;

public class InvoicingEndpointSurfaceTests
{
    [Fact]
    public void MapInvoicingEndpoints_ExposeInvoiceAndSettingsRoutes()
    {
        using var app = CreateApp();

        app.MapInvoicingEndpoints();

        var patterns = GetRoutePatterns(app);

        Assert.Contains("/invoices", patterns);
        Assert.Contains("/invoices/{id}", patterns);
        Assert.Contains("/invoices/{id}/issue", patterns);
        Assert.Contains("/invoices/{id}/cancel", patterns);
        Assert.Contains("/invoicing/settings", patterns);
    }

    [Fact]
    public void MapInvoicingEndpoints_RequireAuthorizationOnMutatingRoutes()
    {
        using var app = CreateApp();

        app.MapInvoicingEndpoints();

        var endpoints = GetRouteEndpoints(app).ToList();
        var mutatingEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText is not null &&
                (Normalize(endpoint.RoutePattern.RawText).EndsWith("/invoices", StringComparison.Ordinal) ||
                 Normalize(endpoint.RoutePattern.RawText).EndsWith("/issue", StringComparison.Ordinal) ||
                 Normalize(endpoint.RoutePattern.RawText).EndsWith("/cancel", StringComparison.Ordinal) ||
                 Normalize(endpoint.RoutePattern.RawText).EndsWith("/invoicing/settings", StringComparison.Ordinal)))
            .ToList();

        Assert.NotEmpty(mutatingEndpoints);
        Assert.All(mutatingEndpoints, endpoint => Assert.Contains(endpoint.Metadata, metadata => metadata is Microsoft.AspNetCore.Authorization.IAuthorizeData));
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
        => routePattern.Length > 1 ? routePattern.TrimEnd('/') : routePattern;
}