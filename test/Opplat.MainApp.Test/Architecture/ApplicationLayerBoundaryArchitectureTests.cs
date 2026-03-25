using System.IO;
using Opplat.MainApp.Test.Auth;

namespace Opplat.MainApp.Test.Architecture;

public class ApplicationLayerBoundaryArchitectureTests
{
    [Fact]
    public void SharedApplicationProject_HostsFlattenedSalesAndInventoryHandlerSlices()
    {
        var sharedApplicationProject = TestRepository.ReadAllText("src", "Opplat.Application", "Opplat.Application.csproj");
        var salesRoot = TestRepository.ResolvePath("src", "Opplat.Application", "Sales");
        var inventoryRoot = TestRepository.ResolvePath("src", "Opplat.Application", "Inventory");

        Assert.Contains(@"..\Modules\Sales\Domain\Opplat.Modules.Sales.Domain.csproj", sharedApplicationProject);
        Assert.Contains(@"..\Modules\Sales\Infrastructure\Opplat.Modules.Sales.Infrastructure.csproj", sharedApplicationProject);
        Assert.Contains(@"..\Modules\Inventory\Domain\Opplat.Modules.Inventory.Domain.csproj", sharedApplicationProject);
        Assert.Contains(@"..\Modules\Inventory\Infrastructure\Opplat.Modules.Inventory.Infrastructure.csproj", sharedApplicationProject);

        Assert.True(Directory.Exists(salesRoot));
        Assert.True(Directory.Exists(inventoryRoot));
        Assert.NotEmpty(Directory.GetFiles(salesRoot, "*Requests.cs", SearchOption.AllDirectories));
        Assert.NotEmpty(Directory.GetFiles(inventoryRoot, "*Requests.cs", SearchOption.AllDirectories));

        AssertModuleSourcesStayInOwnNamespace(salesRoot, "Opplat.Application.Sales");
        AssertModuleSourcesStayInOwnNamespace(inventoryRoot, "Opplat.Application.Inventory");
    }

    [Fact]
    public void SharedApplicationProject_OwnsModuleCompositionAndLegacyModuleApplicationProjectsAreGone()
    {
        var salesProject = TestRepository.ResolvePath("src", "Modules", "Sales", "Application", "Opplat.Modules.Sales.Application.csproj");
        var inventoryProject = TestRepository.ResolvePath("src", "Modules", "Inventory", "Application", "Opplat.Modules.Inventory.Application.csproj");
        var salesDependencyInjection = TestRepository.ReadAllText("src", "Opplat.Application", "Sales", "DependencyInjection", "ServiceCollectionExtensions.cs");
        var inventoryDependencyInjection = TestRepository.ReadAllText("src", "Opplat.Application", "Inventory", "DependencyInjection", "ServiceCollectionExtensions.cs");

        Assert.False(File.Exists(salesProject));
        Assert.False(File.Exists(inventoryProject));
        Assert.Contains("namespace Opplat.Application.Sales.DependencyInjection;", salesDependencyInjection);
        Assert.Contains("AddSalesApplication", salesDependencyInjection);
        Assert.Contains("AddScoped<IProductService, ProductService>();", salesDependencyInjection);
        Assert.Contains("AddScoped<ISalesService, SalesService>();", salesDependencyInjection);
        Assert.Contains("namespace Opplat.Application.Inventory.DependencyInjection;", inventoryDependencyInjection);
        Assert.Contains("AddInventoryApplication", inventoryDependencyInjection);
        Assert.Contains("AddScoped<IProductService, ProductService>();", inventoryDependencyInjection);
        Assert.Contains("AddScoped<IInventoryService, InventoryService>();", inventoryDependencyInjection);
    }

    [Fact]
    public void Hosts_RegisterSharedApplicationHandlersFromSharedApplicationProject()
    {
        var mainAppProject = TestRepository.ReadAllText("src", "Opplat.MainApp", "Opplat.MainApp.csproj");
        var mainAppProgram = TestRepository.ReadAllText("src", "Opplat.MainApp", "Program.cs");
        var mainAppSalesEndpoints = TestRepository.ReadAllText("src", "Opplat.MainApp", "Features", "Sales", "SalesEndpoints.cs");
        var mainAppInventoryEndpoints = TestRepository.ReadAllText("src", "Opplat.MainApp", "Features", "Inventory", "InventoryEndpoints.cs");
        var salesApiProject = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Opplat.Services.Sales.Api.csproj");
        var salesApiProgram = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Program.cs");
        var salesApiEndpoints = TestRepository.ReadAllText("src", "Services", "Sales", "Opplat.Services.Sales.Api", "Endpoints", "SalesEndpoints.cs");
        var inventoryApiProject = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Opplat.Services.Inventory.Api.csproj");
        var inventoryApiProgram = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Program.cs");
        var inventoryApiEndpoints = TestRepository.ReadAllText("src", "Services", "Inventory", "Opplat.Services.Inventory.Api", "Endpoints", "InventoryEndpoints.cs");

        Assert.Contains(@"..\Opplat.Application\Opplat.Application.csproj", mainAppProject);
        Assert.DoesNotContain(@"..\Modules\Sales\Application\Opplat.Modules.Sales.Application.csproj", mainAppProject, StringComparison.Ordinal);
        Assert.DoesNotContain(@"..\Modules\Inventory\Application\Opplat.Modules.Inventory.Application.csproj", mainAppProject, StringComparison.Ordinal);
        Assert.Contains("builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());", mainAppProgram);
        Assert.DoesNotContain("typeof(Opplat.Modules.Sales.Application.AssemblyMarker).Assembly", mainAppProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("typeof(Opplat.Modules.Inventory.Application.AssemblyMarker).Assembly", mainAppProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("builder.Services.AddSalesApplication();", mainAppProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("builder.Services.AddInventoryApplication();", mainAppProgram, StringComparison.Ordinal);
        Assert.Contains("using Opplat.Application.Sales.Products;", mainAppSalesEndpoints);
        Assert.Contains("using Opplat.Application.Inventory.Products;", mainAppInventoryEndpoints);
        Assert.DoesNotContain("using Opplat.Modules.Sales.Application.", mainAppSalesEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("using Opplat.Modules.Inventory.Application.", mainAppInventoryEndpoints, StringComparison.Ordinal);

        Assert.Contains(@"..\..\..\Opplat.Application\Opplat.Application.csproj", salesApiProject);
        Assert.DoesNotContain(@"..\..\..\Modules\Sales\Application\Opplat.Modules.Sales.Application.csproj", salesApiProject, StringComparison.Ordinal);
        Assert.Contains("builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());", salesApiProgram);
        Assert.DoesNotContain("builder.Services.AddSalesModuleServices();", salesApiProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("typeof(Opplat.Modules.Sales.Application.AssemblyMarker).Assembly", salesApiProgram, StringComparison.Ordinal);
        Assert.Contains("using Opplat.Application.Sales.Products;", salesApiEndpoints);
        Assert.DoesNotContain("using Opplat.Modules.Sales.Application.", salesApiEndpoints, StringComparison.Ordinal);

        Assert.Contains(@"..\..\..\Opplat.Application\Opplat.Application.csproj", inventoryApiProject);
        Assert.DoesNotContain(@"..\..\..\Modules\Inventory\Application\Opplat.Modules.Inventory.Application.csproj", inventoryApiProject, StringComparison.Ordinal);
        Assert.Contains("builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());", inventoryApiProgram);
        Assert.DoesNotContain("builder.Services.AddInventoryModuleServices();", inventoryApiProgram, StringComparison.Ordinal);
        Assert.DoesNotContain("typeof(Opplat.Modules.Inventory.Application.AssemblyMarker).Assembly", inventoryApiProgram, StringComparison.Ordinal);
        Assert.Contains("using Opplat.Application.Inventory.Products;", inventoryApiEndpoints);
        Assert.DoesNotContain("using Opplat.Modules.Inventory.Application.", inventoryApiEndpoints, StringComparison.Ordinal);
    }

    private static void AssertModuleSourcesStayInOwnNamespace(string applicationRoot, string namespacePrefix)
    {
        var sourceFiles = GetProjectSourceFiles(applicationRoot);
        Assert.NotEmpty(sourceFiles);

        foreach (var sourceFile in sourceFiles)
        {
            var source = File.ReadAllText(sourceFile);

            Assert.Contains($"namespace {namespacePrefix}", source, StringComparison.Ordinal);
            Assert.DoesNotContain("namespace Opplat.Modules.Sales.Application", source, StringComparison.Ordinal);
            Assert.DoesNotContain("namespace Opplat.Modules.Inventory.Application", source, StringComparison.Ordinal);
        }
    }

    private static string[] GetProjectSourceFiles(string projectRoot)
    {
        return Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path =>
                !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                !path.Contains($"{Path.DirectorySeparatorChar}obj-hicks{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
