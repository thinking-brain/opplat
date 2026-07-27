namespace Opplat.Domain.Entities.Invoicing;

public enum InvoicingMode
{
    None,
    Verifactu,
    NonVerifactuSigned
}

public sealed class TenantFiscalSettings : BaseEntity
{
    public required string LegalName { get; set; }

    public required string TaxId { get; set; }

    public required string FiscalAddress { get; set; }

    public string? Country { get; set; }

    public string? BusinessSector { get; set; }

    public required string DefaultSeries { get; set; }

    public InvoicingMode InvoicingMode { get; set; } = InvoicingMode.None;

    public decimal SimplifiedInvoiceThreshold { get; set; } = 400m;

    public string? SoftwareLicenseId { get; set; }
}