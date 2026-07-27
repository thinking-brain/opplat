using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.Infrastructure.DependencyInjection;

public static class InvoicingInfrastructureExtensions
{
    public static IServiceCollection AddInvoicingInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceCounterService, InvoiceCounterService>();
        services.AddScoped<IInvoiceFiscalizationProvider, NullInvoiceFiscalizationProvider>();
        services.AddScoped<IInvoiceTypeResolver, DefaultInvoiceTypeResolver>();

        return services;
    }
}