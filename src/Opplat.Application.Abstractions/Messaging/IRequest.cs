namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Marker interface for a request that is dispatched through <see cref="IMediator"/>
/// and produces no meaningful response (fire-and-forget style commands).
/// </summary>
public interface IRequest;

/// <summary>
/// Marker interface for a request that is dispatched through <see cref="IMediator"/>
/// and produces a <typeparamref name="TResponse"/> result.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public interface IRequest<out TResponse>;
