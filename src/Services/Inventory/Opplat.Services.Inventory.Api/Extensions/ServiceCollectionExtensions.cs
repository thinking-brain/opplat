using Opplat.Application.Inventory.DependencyInjection;

namespace Opplat.Services.Inventory.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInventoryModuleServices(this IServiceCollection services)
    {
        return services.AddInventoryApplication();
    }
}
