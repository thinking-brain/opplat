namespace Opplat.Domain.Entities.Invoicing;

public sealed class InvoiceLine : BaseEntity
{
    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public Guid? ProductId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxRate { get; set; }

    public decimal LineTotal { get; set; }
}