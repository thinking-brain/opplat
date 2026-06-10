using System.Text.RegularExpressions;
using Opplat.UnitTest.Auth;

namespace Opplat.UnitTest.Architecture;

public class ConvertedSurfaceArchitectureTests
{
    [Fact]
    public void MainAppProgram_MapsConvertedFeaturesViaMinimalApiExtensions()
    {
        var program = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Program.cs");

        Assert.Contains("app.MapAdminEndpoints();", program);
        Assert.Contains("app.MapAccountEndpoints();", program);
        Assert.Contains("app.MapInventoryEndpoints();", program);
        Assert.Contains("app.MapLicenseEndpoints();", program);
        Assert.Contains("app.MapMenusEndpoints();", program);
        Assert.Contains("app.MapSalesEndpoints();", program);
        Assert.DoesNotContain("AddControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/auth/account", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPost(\"/auth/account", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/admin/tenants", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPost(\"/admin/tenants", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/admin/menus", program, StringComparison.Ordinal);
        Assert.DoesNotContain("InventoryArea", program, StringComparison.Ordinal);
        Assert.DoesNotContain("tenant-inventory", program, StringComparison.Ordinal);
        Assert.DoesNotContain("SalesArea", program, StringComparison.Ordinal);
        Assert.DoesNotContain("tenant-sales", program, StringComparison.Ordinal);
    }

    [Fact]
    public void MainAppConvertedFeatureEndpoints_StayMediatorBackedAndControllerFree()
    {
        var adminEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Admin", "AdminEndpoints.cs");
        var accountEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Account", "AccountEndpoints.cs");
        var inventoryEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Inventory", "InventoryEndpoints.cs");
        var licenseEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "License", "LicenseEndpoints.cs");
        var menusEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Menus", "MenusEndpoints.cs");
        var salesEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Sales", "SalesEndpoints.cs");

        AssertMinimalEndpointModule(adminEndpoints);
        Assert.Contains("[FromServices] IAntiforgery antiforgery", adminEndpoints);
        Assert.Contains("await mediator.Send(new GetTenantsQuery())", adminEndpoints);
        Assert.Contains("await mediator.Send(new GetAdminUsersQuery(tenantIdentifier))", adminEndpoints);
        Assert.Contains("await mediator.Send(new GetTenantUsersQuery())", adminEndpoints);
        Assert.Contains("await mediator.Send(new CreateTenantUserCommand(", adminEndpoints);
        Assert.Contains("ValidateResolvedTenant(tenantIdentifier, tenantAccessor)", adminEndpoints);
        Assert.DoesNotContain("UserManager<", adminEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("RoleManager<", adminEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("OpplatDbContext", adminEndpoints, StringComparison.Ordinal);

        AssertMinimalEndpointModule(accountEndpoints);
        Assert.Contains("mediator.Send(new GetUsersQuery())", accountEndpoints);
        Assert.Contains("new RegisterUserCommand(", accountEndpoints);

        AssertMinimalEndpointModule(inventoryEndpoints);
        Assert.Contains("[FromServices] IMediator mediator", inventoryEndpoints);
        Assert.Contains("using Opplat.Application.Inventory.Products;", inventoryEndpoints);
        Assert.DoesNotContain("Opplat.Modules.Inventory.Application.", inventoryEndpoints, StringComparison.Ordinal);
        Assert.Contains("app.MapGroup(\"/{__tenant__}/inventory\")", inventoryEndpoints);
        Assert.Contains("mediator.Send(new GetProductQuery(id))", inventoryEndpoints);
        Assert.Contains("mediator.Send(new ListProductsQuery())", inventoryEndpoints);
        Assert.Contains("mediator.Send(new CreateProductCommand(product, GetCurrentUser(httpContext)))", inventoryEndpoints);
        Assert.Contains("mediator.Send(new ListMovementTypesQuery())", inventoryEndpoints);
        Assert.DoesNotContain("IProductService", inventoryEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IInventoryService", inventoryEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IStorageService", inventoryEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IMovementTypeService", inventoryEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductMovementService", inventoryEndpoints, StringComparison.Ordinal);

        AssertMinimalEndpointModule(licenseEndpoints);
        Assert.Contains("mediator.Send(new GetLicenseQuery())", licenseEndpoints);
        Assert.Contains("mediator.Send(new AddLicenseCommand(", licenseEndpoints);
        Assert.Contains("mediator.Send(new DeleteLicenseCommand())", licenseEndpoints);

        AssertMinimalEndpointModule(menusEndpoints);
        Assert.Contains("mediator.Send(new GetMenusQuery(", menusEndpoints);
        Assert.Contains("mediator.Send(new GetModuleMenuQuery(", menusEndpoints);

        AssertMinimalEndpointModule(salesEndpoints);
        Assert.Contains("[FromServices] IMediator mediator", salesEndpoints);
        Assert.Contains("using Opplat.Application.Sales.Products;", salesEndpoints);
        Assert.DoesNotContain("Opplat.Modules.Sales.Application.", salesEndpoints, StringComparison.Ordinal);
        Assert.Contains("app.MapGroup(\"/{__tenant__}/sales\")", salesEndpoints);
        Assert.Contains("mediator.Send(new ListSalesQuery())", salesEndpoints);
        Assert.Contains("mediator.Send(new ListProductsQuery())", salesEndpoints);
        Assert.Contains("mediator.Send(new CreateProductCommand(product, GetCurrentUser(httpContext)))", salesEndpoints);
        Assert.Contains("mediator.Send(new ListToppingsQuery())", salesEndpoints);
        Assert.Contains("mediator.Send(new ListProductTagsQuery())", salesEndpoints);
        Assert.Contains("mediator.Send(new ListCostTabsQuery())", salesEndpoints);
        Assert.DoesNotContain("ISalesService", salesEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductService", salesEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IToppingService", salesEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductTagService", salesEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("ICostTabService", salesEndpoints, StringComparison.Ordinal);
    }

    [Fact]
    public void MainAppArchivedControllers_StayUnmappedAfterMinimalApiConversion()
    {
        var accountController = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Controllers", "AccountController.cs");
        var inventoryControllersPath = TestRepository.ResolvePath("src", "Opplat.Api.Main", "Areas", "Inventory", "Controllers");
        var salesControllersPath = TestRepository.ResolvePath("src", "Opplat.Api.Main", "Areas", "Sales", "Controllers");
        var licenseController = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Controllers", "LicenciaController.cs");
        var menusController = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Controllers", "MenusController.cs");

        AssertArchivedController(accountController, "Features/Account/", "AccountController_Archived");
        AssertArchivedInventoryControllers(inventoryControllersPath);
        AssertArchivedSalesControllers(salesControllersPath);
        AssertArchivedController(licenseController, "Features/License/", "LicenciaController_Archived");
        AssertArchivedController(menusController, "Features/Menus/", "MenusController_Archived");
    }

    [Fact]
    public void MainAppControllerSources_DoNotContainAnyLiveControllerMappingMarkers()
    {
        var controllerRoots = new[]
        {
            TestRepository.ResolvePath("src", "Opplat.Api.Main", "Controllers"),
            TestRepository.ResolvePath("src", "Opplat.Api.Main", "Areas")
        };

        var controllerFiles = controllerRoots
            .Where(Directory.Exists)
            .SelectMany(root => Directory.GetFiles(root, "*Controller*.cs", SearchOption.AllDirectories))
            .ToList();

        Assert.NotEmpty(controllerFiles);

        foreach (var controllerFile in controllerFiles)
        {
            var source = File.ReadAllText(controllerFile);
            var liveApiControllerAttribute = Regex.IsMatch(source, @"(?m)^(?!\s*//)\s*\[ApiController\]");
            var liveControllerClass = Regex.IsMatch(source, @"(?m)^(?!\s*//)\s*public\s+class\s+\w*Controller(?!_Archived)\b");

            Assert.False(liveApiControllerAttribute, $"Found a live [ApiController] attribute in {controllerFile}.");
            Assert.False(liveControllerClass, $"Found a live controller class in {controllerFile}.");
        }
    }

    [Fact]
    public void AdminApiHost_StaysThinAndControllerFree()
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

        AssertMinimalEndpointModule(endpoints);
        Assert.Contains("[FromServices] IMediator mediator", endpoints);
        Assert.DoesNotContain("AdminTenantCatalogDbContext", endpoints, StringComparison.Ordinal);
    }

    private static void AssertMinimalEndpointModule(string source)
    {
        Assert.Contains("MapGet(", source);
        Assert.DoesNotContain("[ApiController]", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ControllerBase", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddControllers", source, StringComparison.Ordinal);
    }

    private static void AssertArchivedController(string source, string replacementPath, string archivedClassName)
    {
        Assert.Contains(replacementPath, source);
        Assert.Contains(archivedClassName, source);
        Assert.Contains("// [ApiController]", source);
        Assert.DoesNotContain($"class {archivedClassName.Replace("_Archived", string.Empty)} :", source, StringComparison.Ordinal);
    }

    private static void AssertArchivedInventoryControllers(string controllersPath)
    {
        if (!Directory.Exists(controllersPath))
        {
            return;
        }

        var controllerFiles = Directory.GetFiles(controllersPath, "*.cs", SearchOption.TopDirectoryOnly);
        Assert.NotEmpty(controllerFiles);

        foreach (var controllerFile in controllerFiles)
        {
            var source = File.ReadAllText(controllerFile);
            var controllerName = Path.GetFileNameWithoutExtension(controllerFile);

            Assert.Contains("Features/Inventory/InventoryEndpoints.cs", source);
            Assert.Contains("_Archived", source);
            Assert.Contains("// [Route", source);
            Assert.DoesNotContain($"public class {controllerName} : ControllerBase", source, StringComparison.Ordinal);
        }
    }

    private static void AssertArchivedSalesControllers(string controllersPath)
    {
        if (!Directory.Exists(controllersPath))
        {
            return;
        }

        var controllerFiles = Directory.GetFiles(controllersPath, "*.cs", SearchOption.TopDirectoryOnly);
        Assert.NotEmpty(controllerFiles);

        foreach (var controllerFile in controllerFiles)
        {
            var source = File.ReadAllText(controllerFile);
            var controllerName = Path.GetFileNameWithoutExtension(controllerFile);

            Assert.Contains("Features/Sales/SalesEndpoints.cs", source);
            Assert.Contains("_Archived", source);
            Assert.True(
                source.Contains("// [Route", StringComparison.Ordinal) ||
                source.Contains("// [Area", StringComparison.Ordinal) ||
                source.Contains("// [Authorize]", StringComparison.Ordinal),
                $"Expected archived routing/authorization markers in {controllerName}.");
            Assert.DoesNotContain($"public class {controllerName} : ControllerBase", source, StringComparison.Ordinal);
        }
    }
}
