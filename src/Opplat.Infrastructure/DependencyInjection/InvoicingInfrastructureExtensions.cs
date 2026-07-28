using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.Infrastructure.DependencyInjection;

public static class InvoicingInfrastructureExtensions
{
    public static IServiceCollection AddInvoicingInfrastructure(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        services.AddScoped<IInvoiceCounterService, InvoiceCounterService>();
        services.AddScoped<IInvoiceFiscalizationProvider, NullInvoiceFiscalizationProvider>();
        services.AddScoped<IInvoiceTypeResolver, DefaultInvoiceTypeResolver>();

        if (configuration is not null)
            services.Configure<VerifactuWorkerOptions>(
                configuration.GetSection(VerifactuWorkerOptions.SectionName));
        else
            services.Configure<VerifactuWorkerOptions>(_ => { });

        services.AddHostedService<VerifactuSubmissionWorker>();

        return services;
    }
}