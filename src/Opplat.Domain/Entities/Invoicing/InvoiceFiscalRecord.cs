namespace Opplat.Domain.Entities.Invoicing;

public enum FiscalSubmissionMode
{
    Verifactu,
    NonVerifactuSigned,
    None
}

public enum FiscalSubmissionStatus
{
    NotApplicable,
    Pending,
    Submitted,
    Accepted,
    Rejected
}

public sealed class InvoiceFiscalRecord : BaseEntity
{
    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    /// <summary>Position in the issuer's hash chain; assigned under a lock so it is gapless.</summary>
    public long ChainSequence { get; set; }

    /// <summary>NIF of the issuer, kept on the record because it is part of the hash and the AEAT submission.</summary>
    public string IssuerTaxId { get; set; } = string.Empty;

    public string? PreviousRecordHash { get; set; }

    public required string RecordHash { get; set; }

    public required string HashInput { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public required string SoftwareName { get; set; }

    public required string SoftwareVersion { get; set; }

    public string? SoftwareLicenseId { get; set; }

    public FiscalSubmissionMode SubmissionMode { get; set; }

    public FiscalSubmissionStatus SubmissionStatus { get; set; }

    public string? AeatCsv { get; set; }

    public DateTime? AeatSubmittedAtUtc { get; set; }

    public string? AeatResponseRaw { get; set; }

    public string? QrCodePayload { get; set; }

    public string? SignatureValue { get; set; }

    // Transactional outbox retry state
    public int RetryCount { get; set; }

    public DateTime? NextRetryAtUtc { get; set; }

    public string? LastErrorMessage { get; set; }
}