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
}