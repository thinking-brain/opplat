namespace Opplat.Application.Inventory.Common;

public sealed record InventoryCommandResult(bool Succeeded, string Message, IReadOnlyCollection<string> Errors)
{
    public static InventoryCommandResult From(bool succeeded, string message)
        => new(succeeded, message, succeeded ? [] : [message]);
}

