namespace Opplat.Domain.Entities.Invoicing;

public sealed class Customer : BaseEntity
{
    public required string Name { get; set; }

    public string? TaxId { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public bool IsFinalConsumer { get; set; }
}