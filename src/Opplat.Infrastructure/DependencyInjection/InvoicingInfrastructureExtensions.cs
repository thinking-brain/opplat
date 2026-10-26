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
        services.AddScoped<IInvoiceSigningService, XadesInvoiceSigningService>();

        // The fiscalization provider is picked per call from TenantFiscalSettings.InvoicingMode.
        services.AddScoped<NullInvoiceFiscalizationProvider>();
        services.AddScoped<VerifactuFiscalizationProvider>();
        services.AddScoped<IInvoiceFiscalizationProvider, ModeAwareInvoiceFiscalizationProvider>();
        services.AddScoped<VerifactuSubmissionClient>();

        if (configuration is not null)
        {
            services.Configure<XadesSigningOptions>(
                configuration.GetSection(XadesSigningOptions.SectionName));
            services.Configure<VerifactuWorkerOptions>(
                configuration.GetSection(VerifactuWorkerOptions.SectionName));
            services.Configure<VerifactuSubmissionOptions>(
                configuration.GetSection(VerifactuSubmissionOptions.SectionName));
        }
        else
        {
            services.Configure<XadesSigningOptions>(_ => { });
            services.Configure<VerifactuWorkerOptions>(_ => { });
            services.Configure<VerifactuSubmissionOptions>(_ => { });
        }

        services.AddHostedService<VerifactuSubmissionWorker>();

        return services;
    }
}