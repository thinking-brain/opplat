using System.IO;
using Opplat.MainApp.Test.Auth;

namespace Opplat.MainApp.Test.Architecture;

public class MicroserviceThinHostArchitectureTests
{
    [Fact]
    public void SalesApiHost_StaysThinAndRoutesEndpointLogicThroughMediatR()
    {
        var program = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Program.cs");
        var endpoints = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Endpoints", "SalesEndpoints.cs");
        var hostRegistration = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Extensions", "ServiceCollectionExtensions.cs");

        AssertThinHostProgram(
            program,
            "app.MapSalesEndpoints();",
            "builder.Services.AddOpplatApplication(");
        Assert.Contains("return services.AddSalesApplication();", hostRegistration);

        Assert.Contains("[FromServices] IMediator mediator", endpoints);
        Assert.Contains("using Opplat.Application.Sales.Products;", endpoints);
        Assert.DoesNotContain("Opplat.Modules.Sales.Application.", endpoints, StringComparison.Ordinal);
        Assert.Contains("mediator.Send(", endpoints);
        Assert.DoesNotContain("ISalesService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IToppingService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductTagService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("ICostTabService", endpoints, StringComparison.Ordinal);
    }

    [Fact]
    public void InventoryApiHost_StaysThinAndRoutesEndpointLogicThroughMediatR()
    {
        var program = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Program.cs");
        var endpoints = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Endpoints", "InventoryEndpoints.cs");
        var hostRegistration = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Extensions", "ServiceCollectionExtensions.cs");

        AssertThinHostProgram(
            program,
            "app.MapInventoryEndpoints();",
            "builder.Services.AddOpplatApplication(");
        Assert.Contains("return services.AddInventoryApplication();", hostRegistration);

        Assert.Contains("[FromServices] IMediator mediator", endpoints);
        Assert.Contains("using Opplat.Application.Inventory.Products;", endpoints);
        Assert.DoesNotContain("Opplat.Modules.Inventory.Application.", endpoints, StringComparison.Ordinal);
        Assert.Contains("mediator.Send(", endpoints);
        Assert.DoesNotContain("IInventoryService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductClassificationService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductGroupService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IStorageService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IMovementTypeService", endpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("IProductMovementService", endpoints, StringComparison.Ordinal);
    }

    [Fact]
    public void InventoryControllers_AreArchivedOrRemovedAfterHandlerMigration()
    {
        var controllersPath = TestRepository.ResolvePath("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Controllers");

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

            Assert.Contains("_Archived", source);
            Assert.Contains("Endpoints/InventoryEndpoints.cs", source);
            Assert.Contains("// [Route", source);
            Assert.DoesNotContain($"public class {controllerName} : ControllerBase", source, StringComparison.Ordinal);
        }
    }

    private static void AssertThinHostProgram(
        string program,
        string endpointMapping,
        string applicationRegistration)
    {
        Assert.Contains("builder.Services.AddOpplatMicroserviceHost", program);
        Assert.Contains(applicationRegistration, program);
        Assert.Contains("app.UseOpplatMicroserviceHost();", program);
        Assert.Contains(endpointMapping, program);
        Assert.DoesNotContain("AddControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapControllers", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/sales", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPost(\"/sales", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPut(\"/sales", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapDelete(\"/sales", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapGet(\"/inventory", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPost(\"/inventory", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPut(\"/inventory", program, StringComparison.Ordinal);
        Assert.DoesNotContain("MapDelete(\"/inventory", program, StringComparison.Ordinal);
    }
}
