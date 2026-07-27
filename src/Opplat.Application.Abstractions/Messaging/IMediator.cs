namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Dispatches requests to their registered <see cref="IRequestHandler{TRequest}"/> or
/// <see cref="IRequestHandler{TRequest, TResponse}"/> implementation.
/// This is a lightweight, in-process replacement for MediatR's <c>IMediator</c>.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request that produces a <typeparamref name="TResponse"/> result to its handler.
    /// </summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request that produces no meaningful response to its handler.
    /// </summary>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
