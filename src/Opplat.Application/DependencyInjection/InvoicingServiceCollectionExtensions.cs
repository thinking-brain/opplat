using Microsoft.Extensions.DependencyInjection;

namespace Opplat.Application.DependencyInjection;

public static class InvoicingServiceCollectionExtensions
{
    public static IServiceCollection AddInvoicingApplication(this IServiceCollection services)
    {
        return services;
    }
}