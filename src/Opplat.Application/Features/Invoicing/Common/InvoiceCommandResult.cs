namespace Opplat.Application.Features.Invoicing.Common;

public sealed record InvoiceCommandResult(bool Succeeded, string Message, IReadOnlyCollection<string> Errors)
{
    public static InvoiceCommandResult From(bool succeeded, string message)
        => new(succeeded, message, succeeded ? [] : [message]);
}