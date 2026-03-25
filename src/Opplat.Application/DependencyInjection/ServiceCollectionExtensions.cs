using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Inventory.DependencyInjection;
using Opplat.Application.Sales.DependencyInjection;

namespace Opplat.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpplatApplication(
        this IServiceCollection services,
        params Assembly[] additionalAssemblies)
    {
        var assemblies = new[] { typeof(AssemblyMarker).Assembly }
            .Concat(additionalAssemblies)
            .Distinct()
            .ToArray();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        services.AddSalesApplication();
        services.AddInventoryApplication();

        return services;
    }
}
