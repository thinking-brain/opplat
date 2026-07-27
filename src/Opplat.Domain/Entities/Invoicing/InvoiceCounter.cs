namespace Opplat.Domain.Entities.Invoicing;

public sealed class InvoiceCounter : BaseEntity
{
    public required string Series { get; set; }

    public int Year { get; set; }

    public int LastNumber { get; set; }
}