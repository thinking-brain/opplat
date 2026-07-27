namespace Opplat.Application.Abstractions.Messaging;

/// <summary>
/// Represents a void type for <see cref="ICommand"/>/<see cref="ICommandHandler{TCommand}"/>
/// contracts, mirroring MediatR's <c>Unit</c> so commands can be modeled as
/// <see cref="IRequest{TResponse}"/> even when there is no meaningful result.
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default;

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public override int GetHashCode() => 0;

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;
}
