using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Identity;
using Opplat.Infrastructure.Services.Billing;

namespace Opplat.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentGatewayService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(StripeOptions.SectionName);
        services.Configure<StripeOptions>(section);
        var options = section.Get<StripeOptions>() ?? new StripeOptions();

        services.AddScoped<IPaymentGatewayService, NoOpPaymentGatewayService>();

        if (options.Enabled)
            throw new InvalidOperationException("Stripe payment gateway is enabled but its implementation is not configured yet.");

        return services;
    }

    /// <summary>
    /// Registers Graph API user-lifecycle services.
    /// When <c>GraphApi:Enabled</c> is true, wires the real Graph SDK client with
    /// client-credentials flow.
    /// Otherwise, registers a no-op stub for local development.
    /// </summary>
    public static IServiceCollection AddGraphUserService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(GraphApiOptions.SectionName);
        services.Configure<GraphApiOptions>(section);

        var options = section.Get<GraphApiOptions>() ?? new GraphApiOptions();

        if (options.Enabled
            && !string.IsNullOrWhiteSpace(options.TenantId)
            && !string.IsNullOrWhiteSpace(options.ClientId))
        {
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<GraphApiOptions>>().Value;

                var credential = !string.IsNullOrWhiteSpace(opts.ClientSecret)
                    ? new ClientSecretCredential(opts.TenantId, opts.ClientId, opts.ClientSecret)
                    : throw new InvalidOperationException(
                        "GraphApi:ClientSecret is required. Certificate-based auth is planned but not yet implemented.");

                return new GraphServiceClient(credential, ["https://graph.microsoft.com/.default"]);
            });

            services.AddScoped<IUserManagementService, GraphUserService>();
        }
        else
        {
            services.AddScoped<IUserManagementService, NoOpGraphUserService>();
        }

        return services;
    }

    public static IServiceCollection AddKeycloakUserService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(KeycloakAdminOptions.SectionName);
        services.Configure<KeycloakAdminOptions>(section);

        var options = section.Get<KeycloakAdminOptions>() ?? new KeycloakAdminOptions();

        if (options.Enabled
            && !string.IsNullOrWhiteSpace(options.BaseUrl)
            && !string.IsNullOrWhiteSpace(options.Realm))
        {
            services.AddHttpClient<KeycloakUserService>();
            services.AddScoped<IUserManagementService, KeycloakUserService>();
        }
        else
        {
            services.AddScoped<IUserManagementService, NoOpKeycloakUserService>();
        }

        return services;
    }
}
