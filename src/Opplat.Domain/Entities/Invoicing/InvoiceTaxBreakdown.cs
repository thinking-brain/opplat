namespace Opplat.Domain.Entities.Invoicing;

public enum TaxType
{
    IVA,
    IGIC,
    IPSI,
    Exempt
}

public sealed class InvoiceTaxBreakdown : BaseEntity
{
    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public TaxType TaxType { get; set; }

    public decimal Rate { get; set; }

    public decimal TaxableBase { get; set; }

    public decimal TaxAmount { get; set; }
}