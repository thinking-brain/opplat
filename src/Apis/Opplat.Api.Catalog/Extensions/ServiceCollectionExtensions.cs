namespace Opplat.Api.Catalog.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Catalog domain services.
    /// When running as a standalone microservice, also call <c>AddOpplatApplication()</c>
    /// and <c>AddAdminInfrastructure()</c> to wire up MediatR handlers and repositories.
    /// </summary>
    public static IServiceCollection AddCatalogDomain(this IServiceCollection services)
    {
        return services;
    }
}
