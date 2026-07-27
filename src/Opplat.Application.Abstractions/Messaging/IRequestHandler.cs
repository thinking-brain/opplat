namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Handles a <typeparamref name="TRequest"/> that produces no meaningful response.
/// </summary>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handles a <typeparamref name="TRequest"/> and produces a <typeparamref name="TResponse"/> result.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
