namespace Opplat.Api.Inventory.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Inventory domain services.
    /// When running as a standalone microservice, also call <c>AddOpplatApplication()</c>
    /// and infrastructure setup to wire up MediatR handlers and repositories.
    /// </summary>
    public static IServiceCollection AddInventoryDomain(this IServiceCollection services)
    {
        return services;
    }
}
