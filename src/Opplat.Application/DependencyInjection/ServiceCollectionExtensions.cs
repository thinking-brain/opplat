using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Features.Account.Commands;

namespace Opplat.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpplatApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ChangePasswordCommand).Assembly));
        services.AddSalesApplication();
        services.AddInventoryApplication();

        return services;
    }
}
