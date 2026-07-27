using System.Collections.Concurrent;
using System.Reflection;

namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Default <see cref="IMediator"/> implementation. Resolves the handler registered for
/// a request's runtime type from <see cref="IServiceProvider"/> and invokes it.
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethodCache = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for request '{requestType.FullName}'.");

        var method = HandlerMethodCache.GetOrAdd(handlerType, static t => t.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle))!);

        return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for request '{requestType.FullName}'.");

        var method = HandlerMethodCache.GetOrAdd(handlerType, static t => t.GetMethod(nameof(IRequestHandler<IRequest>.Handle))!);

        return (Task)method.Invoke(handler, [request, cancellationToken])!;
    }
}
