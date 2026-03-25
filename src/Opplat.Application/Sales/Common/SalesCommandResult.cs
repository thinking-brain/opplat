namespace Opplat.Application.Sales.Common;

public sealed record SalesCommandResult(bool Succeeded, string Message, IReadOnlyCollection<string> Errors)
{
    public static SalesCommandResult From(bool succeeded, string message)
        => new(succeeded, message, succeeded ? [] : [message]);
}

