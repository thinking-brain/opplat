using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;

namespace Opplat.Infrastructure.DependencyInjection;

public static class InvoicingInfrastructureExtensions
{
    public static IServiceCollection AddInvoicingInfrastructure(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        services.AddScoped<IInvoiceCounterService, InvoiceCounterService>();
        services.AddScoped<IInvoiceTypeResolver, DefaultInvoiceTypeResolver>();
        services.AddScoped<IInvoicePdfRenderer, InvoicePdfRenderer>();
        services.AddScoped<IInvoiceSigningService, NotImplementedInvoiceSigningService>();

        // Register the fiscalization provider that matches the tenant's InvoicingMode.
        // The NullFiscalizationProvider is the safe default for non-Spain / non-Verifactu tenants.
        // Swap to VerifactuFiscalizationProvider by changing this registration (or use a factory
        // that reads TenantFiscalSettings.InvoicingMode at runtime).
        services.AddScoped<IInvoiceFiscalizationProvider, NullInvoiceFiscalizationProvider>();
        services.AddScoped<VerifactuFiscalizationProvider>();
        services.AddScoped<VerifactuSubmissionClient>();

        if (configuration is not null)
        {
            services.Configure<VerifactuWorkerOptions>(
                configuration.GetSection(VerifactuWorkerOptions.SectionName));
            services.Configure<VerifactuSubmissionOptions>(
                configuration.GetSection(VerifactuSubmissionOptions.SectionName));
        }
        else
        {
            services.Configure<VerifactuWorkerOptions>(_ => { });
            services.Configure<VerifactuSubmissionOptions>(_ => { });
        }

        services.AddHostedService<VerifactuSubmissionWorker>();

        return services;
    }
}