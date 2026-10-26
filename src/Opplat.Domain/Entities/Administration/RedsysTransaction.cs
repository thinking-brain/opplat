namespace Opplat.Domain.Entities.Administration;

public enum RedsysTransactionKind
{
    CardSetup,
    Recurring
}

public enum RedsysTransactionStatus
{
    Pending,
    Authorized,
    Declined,
    Error
}

/// <summary>
/// Audit row for every Redsys request/response, keyed by the Ds_Order we generate. Also the
/// correlation mechanism for the async card-setup webhook and the source of the last successful
/// Ds_Merchant_COF_TXNID needed to chain the next MIT (recurring) charge.
/// </summary>
public sealed class RedsysTransaction : BaseEntity
{
    public Guid TenantId { get; set; }

    public string Order { get; set; } = string.Empty;

    public RedsysTransactionKind Kind { get; set; }

    public RedsysTransactionStatus Status { get; set; } = RedsysTransactionStatus.Pending;

    public string? ResponseCode { get; set; }

    public string? CofTxnId { get; set; }

    public string? IdentifierToken { get; set; }

    public long AmountCents { get; set; }

    public string Currency { get; set; } = "EUR";

    public string? RawResponsePayload { get; set; }

    public Tenant? Tenant { get; set; }
}
