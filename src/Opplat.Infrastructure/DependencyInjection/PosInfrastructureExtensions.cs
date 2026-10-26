using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Pos;
using Opplat.Infrastructure.Services.Pos;

namespace Opplat.Infrastructure.DependencyInjection;

public static class PosInfrastructureExtensions
{
    public static IServiceCollection AddPosInfrastructure(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        if (configuration is not null)
        {
            services.Configure<PosPaymentOptions>(configuration.GetSection(PosPaymentOptions.SectionName));
        }
        else
        {
            services.Configure<PosPaymentOptions>(_ => { });
        }

        services.AddSingleton(TimeProvider.System);

        // Singleton: the simulator keeps its terminal state in memory across requests.
        services.AddSingleton<IPaymentTerminalProvider, MockPaymentTerminalProvider>();
        services.AddSingleton<IPaymentTerminalProviderRegistry, PaymentTerminalProviderRegistry>();
        return services;
    }
}
