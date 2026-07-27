using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Registration helpers for the custom <see cref="IMediator"/> implementation.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IMediator"/> and scans the given assemblies for
    /// <see cref="IRequestHandler{TRequest}"/> / <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// implementations, registering each as transient.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.TryAddTransient<IMediator, Mediator>();

        foreach (var assembly in assemblies)
            services.AddRequestHandlersFromAssembly(assembly);

        return services;
    }

    /// <summary>
    /// Registers every <see cref="IRequestHandler{TRequest}"/> / <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// implementation found in <paramref name="assembly"/> as transient, optionally restricted by <paramref name="predicate"/>.
    /// </summary>
    public static IServiceCollection AddRequestHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        Func<Type, bool>? predicate = null)
    {
        var handlerTypes = assembly.GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => predicate is null || predicate(t));

        foreach (var handlerType in handlerTypes)
        {
            foreach (var handlerInterface in GetHandlerInterfaces(handlerType))
                services.AddTransient(handlerInterface, handlerType);
        }

        return services;
    }

    private static IEnumerable<Type> GetHandlerInterfaces(Type type) =>
        type.GetInterfaces()
            .Where(i => i.IsGenericType
                && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));
}
