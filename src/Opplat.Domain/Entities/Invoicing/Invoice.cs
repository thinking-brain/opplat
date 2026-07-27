using Opplat.Domain.Entities.Sales;

namespace Opplat.Domain.Entities.Invoicing;

public enum InvoiceType
{
    Simplified,
    Full,
    Rectifying,
    Substitutive
}

public enum InvoiceStatus
{
    Draft,
    Issued,
    Sent,
    Cancelled
}

public sealed class CustomerSnapshot
{
    public required string Name { get; set; }

    public string? TaxId { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public bool IsFinalConsumer { get; set; }
}

public sealed class Invoice : BaseEntity
{
    public required string Series { get; set; }

    public int Number { get; set; }

    public required string FullNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public InvoiceType InvoiceType { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public Guid? SaleId { get; set; }

    public Sale? Sale { get; set; }

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public CustomerSnapshot CustomerSnapshot { get; set; } = new()
    {
        Name = string.Empty
    };

    public string Currency { get; set; } = "EUR";

    public decimal Subtotal { get; set; }

    public decimal TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; } = new HashSet<InvoiceLine>();

    public ICollection<InvoiceTaxBreakdown> TaxBreakdowns { get; set; } = new HashSet<InvoiceTaxBreakdown>();

    public InvoiceFiscalRecord? FiscalRecord { get; set; }
}