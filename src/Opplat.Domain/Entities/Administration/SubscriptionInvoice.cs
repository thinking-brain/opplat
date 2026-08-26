namespace Opplat.Domain.Entities.Administration;

public enum SubscriptionInvoiceStatus
{
    Paid,
    Open,
    Failed,
    Void
}

public sealed class SubscriptionInvoice : BaseEntity
{
    public Guid TenantId { get; set; }

    public string StripeInvoiceId { get; set; } = string.Empty;

    public decimal AmountDue { get; set; }

    public string Currency { get; set; } = "EUR";

    public SubscriptionInvoiceStatus Status { get; set; }

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? HostedInvoiceUrl { get; set; }

    public Tenant? Tenant { get; set; }
}