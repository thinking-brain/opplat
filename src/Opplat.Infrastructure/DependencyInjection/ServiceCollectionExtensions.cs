using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Identity;

namespace Opplat.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
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

                return new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
            });

            services.AddScoped<IGraphUserService, GraphUserService>();
        }
        else
        {
            services.AddScoped<IGraphUserService, NoOpGraphUserService>();
        }

        return services;
    }
}
