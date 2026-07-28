using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Application.Services;

namespace Opplat.Application.DependencyInjection;

public static class InvoicingServiceCollectionExtensions
{
    public static IServiceCollection AddInvoicingApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaxCalculationService, TaxCalculationService>();
        services.AddScoped<IInvoiceEmailService, InvoiceEmailService>();
        return services;
    }
}