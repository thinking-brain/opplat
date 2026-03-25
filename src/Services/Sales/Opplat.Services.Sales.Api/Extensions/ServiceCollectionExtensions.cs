using Opplat.Application.Sales.DependencyInjection;

namespace Opplat.Services.Sales.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSalesModuleServices(this IServiceCollection services)
    {
        return services.AddSalesApplication();
    }
}
