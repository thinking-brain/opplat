using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Dtos;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.UnitTest.Auth;

public class AdminApiMinimalEndpointContractTests
{
    [Fact]
    public void AdminApiRouteSurface_ExposesTenantCatalogEndpointsWithoutAdminUserCrud()
    {
        using var app = CreateApp();

        Opplat.Api.Admin.Endpoints.AdminEndpoints.MapAdminEndpoints(app);

        var endpoints = GetRouteEndpoints(app).ToList();

        AssertRoute(endpoints, "/admin/tenants", "GET");
        AssertRoute(endpoints, "/admin/tenants", "POST");
        AssertRoute(endpoints, "/admin/tenants/{identifier}", "PUT");
        AssertRoute(endpoints, "/admin/tenants/{identifier}", "DELETE");
        Assert.DoesNotContain(endpoints, endpoint => MatchesRoute(endpoint, "/admin/users", "GET"));
        Assert.DoesNotContain(endpoints, endpoint => Normalize(endpoint.RoutePattern.RawText ?? string.Empty).Contains("/users", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MinimalAdminApiEndpoints_ReturnTenantMetadataWithoutConnectionStrings()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var tenants = await client.GetFromJsonAsync<List<AdminTenantDto>>("/admin/tenants");

        Assert.NotNull(tenants);
        Assert.NotEmpty(tenants);
        Assert.All(tenants, tenant =>
        {
            Assert.False(tenant.Id == Guid.Empty);
            Assert.False(string.IsNullOrWhiteSpace(tenant.Identifier));
            Assert.False(string.IsNullOrWhiteSpace(tenant.Name));
            Assert.False(string.IsNullOrWhiteSpace(tenant.DatabaseName));
            Assert.False(string.IsNullOrWhiteSpace(tenant.DatabaseSchema));
            Assert.True(tenant.UserCount >= 0);
        });
    }

    [Fact]
    public async Task MinimalAdminApiEndpoints_SerializeTenantMetadataInAdminClientJsonShape()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var response = await client.GetAsync("/admin/tenants");
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(responseStream);
        var tenant = document.RootElement.EnumerateArray().First();

        Assert.True(tenant.TryGetProperty("databaseName", out _));
        Assert.True(tenant.TryGetProperty("databaseSchema", out _));
        Assert.True(tenant.TryGetProperty("userCount", out _));
        Assert.True(tenant.TryGetProperty("isActive", out _));
        Assert.False(tenant.TryGetProperty("connectionString", out _));
    }

    [Fact]
    public void AdminTenantContracts_PinMetadataAndExcludeAdminUserCrud()
    {
        var contracts = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Endpoints", "AdminContracts.cs");
        var endpoints = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Endpoints", "AdminEndpoints.cs");

        Assert.Contains("public string DatabaseName", contracts);
        Assert.Contains("public string DatabaseSchema", contracts);
        Assert.Contains("public int UserCount", contracts);
        Assert.DoesNotContain("ConnectionString", contracts, StringComparison.Ordinal);
        Assert.DoesNotContain("AdminUserDto", contracts, StringComparison.Ordinal);
        Assert.DoesNotContain("AdminCreateUserRequest", contracts, StringComparison.Ordinal);
        Assert.DoesNotContain("/admin/users", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("/users/{userId}", endpoints, StringComparison.Ordinal);
    }

    [Fact]
    public void AdminApiHost_UsesEndpointModulesInsteadOfMvcControllers()
    {
        var program = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Program.cs");
        var endpoints = TestRepository.ReadAllText("src", "Opplat.Api.Admin", "Endpoints", "AdminEndpoints.cs");

        Assert.Contains("app.MapGeneralEndpoints();", program);
        Assert.Contains("app.MapAdminEndpoints();", program);
        Assert.DoesNotContain("AddControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/admin", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPost(\"/admin", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPut(\"/admin", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapDelete(\"/admin", program, StringComparison.Ordinal);
        Assert.False(Directory.Exists(TestRepository.ResolvePath("src", "Opplat.Api.Admin", "Controllers")));

        Assert.Contains("[FromServices] IMediator mediator", endpoints);
        Assert.DoesNotContain("ControllerBase", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("AdminTenantCatalogDbContext", endpoints, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MinimalAdminApiWriteFlow_SupportsTenantMetadataCrudWithoutOwningUsers()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var createTenantResponse = await client.PostAsJsonAsync("/admin/tenants", new UpsertTenantRequest
        {
            Identifier = "regression-cafe",
            Name = "Regression Cafe",
            DatabaseName = "opplat_regression_cafe",
            DatabaseSchema = "tenant_regression_cafe",
            IsActive = true
        });
        var createdTenant = await createTenantResponse.Content.ReadFromJsonAsync<AdminTenantDto>();

        Assert.Equal(HttpStatusCode.Created, createTenantResponse.StatusCode);
        Assert.NotNull(createdTenant);
        Assert.Equal("regression-cafe", createdTenant.Identifier);
        Assert.Equal("Regression Cafe", createdTenant.Name);
        Assert.Equal("opplat_regression_cafe", createdTenant.DatabaseName);
        Assert.Equal("tenant_regression_cafe", createdTenant.DatabaseSchema);
        Assert.Equal(0, createdTenant.UserCount);
        Assert.True(createdTenant.IsActive);

        var updateTenantResponse = await client.PutAsJsonAsync("/admin/tenants/regression-cafe", new UpsertTenantRequest
        {
            Id = createdTenant.Id,
            Identifier = "regression-cafe",
            Name = "Regression Cafe HQ",
            DatabaseName = "opplat_regression_cafe_v2",
            DatabaseSchema = "tenant_regression_hq",
            IsActive = true
        });
        var updatedTenant = await updateTenantResponse.Content.ReadFromJsonAsync<AdminTenantDto>();

        Assert.Equal(HttpStatusCode.OK, updateTenantResponse.StatusCode);
        Assert.NotNull(updatedTenant);
        Assert.Equal("Regression Cafe HQ", updatedTenant.Name);
        Assert.Equal("opplat_regression_cafe_v2", updatedTenant.DatabaseName);
        Assert.Equal("tenant_regression_hq", updatedTenant.DatabaseSchema);
        Assert.Equal(0, updatedTenant.UserCount);

        var userCrudResponse = await client.GetAsync("/admin/users");
        Assert.Equal(HttpStatusCode.NotFound, userCrudResponse.StatusCode);

        var deactivateTenantResponse = await client.DeleteAsync("/admin/tenants/regression-cafe");
        Assert.Equal(HttpStatusCode.NoContent, deactivateTenantResponse.StatusCode);

        var refreshedTenants = await client.GetFromJsonAsync<List<AdminTenantDto>>("/admin/tenants");
        var deactivatedTenant = Assert.Single(refreshedTenants!, tenant => tenant.Identifier == "regression-cafe");
        Assert.False(deactivatedTenant.IsActive);
    }

    [Fact]
    public async Task MinimalAdminApiWriteFlow_RejectsDuplicateTenantIdentifiersWithConflict()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/admin/tenants", new UpsertTenantRequest
        {
            Identifier = "mojocafe",
            Name = "MojoCafe Duplicate",
            DatabaseName = "opplat_duplicate",
            DatabaseSchema = "tenant_duplicate",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertErrorMessageAsync(response, "already exists");
    }

    [Fact]
    public async Task MinimalAdminApiWriteFlow_RejectsMissingTenantMetadataWithBadRequest()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/admin/tenants", new UpsertTenantRequest
        {
            Identifier = "   ",
            Name = "Regression Cafe",
            DatabaseName = "opplat_regression",
            DatabaseSchema = "",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertErrorMessageAsync(response, "Identifier is required.");
    }

    [Fact]
    public async Task MinimalAdminApiWriteFlow_RejectsTenantIdMutationWithBadRequest()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var response = await client.PutAsJsonAsync("/admin/tenants/mojocafe", new UpsertTenantRequest
        {
            Id = Guid.NewGuid(),
            Identifier = "mojocafe",
            Name = "MojoCafe",
            DatabaseName = "opplat_mojocafe",
            DatabaseSchema = "tenant_mojocafe",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertErrorMessageAsync(response, "Tenant ID cannot be changed once created.");
    }

    [Fact]
    public async Task MinimalAdminApiWriteFlow_ReturnsNotFoundForUnknownTenantWrites()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        var updateResponse = await client.PutAsJsonAsync("/admin/tenants/missing-tenant", new UpsertTenantRequest
        {
            Identifier = "missing-tenant",
            Name = "Missing Tenant",
            DatabaseName = "opplat_missing",
            DatabaseSchema = "tenant_missing",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        await AssertErrorMessageAsync(updateResponse, "was not found");

        var deleteResponse = await client.DeleteAsync("/admin/tenants/missing-tenant");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        await AssertErrorMessageAsync(deleteResponse, "was not found");
    }

    [Fact]
    public async Task MinimalAdminApiEndpoints_ListTenantsSortedByNameThenIdentifier()
    {
        await using var app = await CreateStartedAppAsync();
        var client = app.GetTestClient();

        await client.PostAsJsonAsync("/admin/tenants", new UpsertTenantRequest
        {
            Identifier = "alpha-z",
            Name = "Alpha Bistro",
            DatabaseName = "opplat_alpha_z",
            DatabaseSchema = "tenant_alpha_z",
            IsActive = true
        });

        await client.PostAsJsonAsync("/admin/tenants", new UpsertTenantRequest
        {
            Identifier = "alpha-a",
            Name = "Alpha Bistro",
            DatabaseName = "opplat_alpha_a",
            DatabaseSchema = "tenant_alpha_a",
            IsActive = true
        });

        var tenants = await client.GetFromJsonAsync<List<AdminTenantDto>>("/admin/tenants");

        Assert.NotNull(tenants);
        Assert.Equal(
            tenants!.OrderBy(tenant => tenant.Name, StringComparer.Ordinal)
                .ThenBy(tenant => tenant.Identifier, StringComparer.Ordinal)
                .Select(tenant => tenant.Identifier),
            tenants.Select(tenant => tenant.Identifier));
    }

    private static WebApplication CreateApp()
    {
        var databaseName = $"admin-api-contract-{Guid.NewGuid():N}";
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseTestServer();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole(AuthRoles.SuperAdmin));
        });
        builder.Services.AddAuthentication();
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
            options.UseInMemoryDatabase(databaseName));
        // builder.Services.Configure<Opplat.Api.Admin.Configuration.DatabaseInstanceOptions>(options =>
        // {
        //     options.DefaultConnectionString = "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*";
        // });
        // builder.Services.AddSingleton(new Opplat.Api.Admin.Configuration.DatabaseInstanceOptions
        // {
        //     DefaultConnectionString = "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*"
        // });
        // builder.Services.AddScoped<Opplat.Api.Admin.Services.TenantSchemaProvisioningService>();
        // builder.Services.AddScoped<Opplat.Api.Admin.Services.DatabaseInstanceAutoScalingService>();
        // builder.Services.AddScoped<Opplat.Api.Admin.Services.TenantSchemaMigrationRunner>();
        // builder.Services.AddScoped<Opplat.Api.Admin.Services.TenantProvisioningCoordinator>();
        // builder.Services.AddSingleton<IEnumerable<Opplat.Api.Admin.Services.ITenantProvisioningReporter>>([]);
        // builder.Services.AddSingleton<IEnumerable<Opplat.Api.Admin.Services.ITenantSchemaMigrationReporter>>([]);
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Opplat.Api.Admin.Endpoints.AdminEndpoints).Assembly));

        return builder.Build();
    }

    private static async Task<WebApplication> CreateStartedAppAsync()
    {
        var app = CreateApp();
        // await AdminPortalDataSeeder.InitializeAsync(app.Services);
        Opplat.Api.Admin.Endpoints.AdminEndpoints.MapAdminEndpoints(app);
        await app.StartAsync();
        return app;
    }

    private static IReadOnlyList<RouteEndpoint> GetRouteEndpoints(WebApplication app)
    {
        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();
    }

    private static void AssertRoute(IEnumerable<RouteEndpoint> endpoints, string routePattern, string httpMethod)
    {
        var endpoint = endpoints.SingleOrDefault(candidate => MatchesRoute(candidate, routePattern, httpMethod));

        Assert.True(endpoint is not null, $"Expected {httpMethod} {routePattern} to be mapped for the admin client contract.");
    }

    private static bool MatchesRoute(RouteEndpoint candidate, string routePattern, string httpMethod) =>
        candidate.RoutePattern.RawText is not null &&
        Normalize(candidate.RoutePattern.RawText!) == routePattern &&
        candidate.Metadata.OfType<IHttpMethodMetadata>()
            .Any(metadata => metadata.HttpMethods.Contains(httpMethod, StringComparer.OrdinalIgnoreCase));

    private static string Normalize(string routePattern) =>
        routePattern.Length > 1 ? routePattern.TrimEnd('/') : routePattern;

    private static async Task AssertErrorMessageAsync(HttpResponseMessage response, string expectedMessageFragment)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Contains(expectedMessageFragment, document.RootElement.GetProperty("message").GetString(), StringComparison.OrdinalIgnoreCase);
    }
}
