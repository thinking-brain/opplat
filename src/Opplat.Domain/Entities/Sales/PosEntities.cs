namespace Opplat.Domain.Entities.Sales;

public enum PosShiftStatus
{
    Open,
    Closed
}

public enum PosCashMovementType
{
    PayIn,
    PayOut
}

public enum SaleStatus
{
    /// <summary>Default so rows created before the POS module read as finished sales.</summary>
    Completed = 0,
    Draft = 1,
    Voided = 2
}

public enum SalePaymentMethod
{
    Cash,
    Card,
    Other
}

public enum SalePaymentStatus
{
    Pending,
    Captured,
    Failed,
    Cancelled
}

public enum PaymentAttemptStatus
{
    Pending,
    Approved,
    Declined,
    Cancelled,
    TimedOut,

    /// <summary>Terminal did not report a result; must be reconciled before the cashier retries.</summary>
    Unknown
}

/// <summary>A till/counter. Owns the invoice series so numbering never contends across registers.</summary>
public class PosRegister : BaseEntity
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string InvoiceSeries { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Inventory storage that stock is taken from when a sale completes.</summary>
    public Guid? StorageId { get; set; }
}

public class PosShift : BaseEntity
{
    public Guid RegisterId { get; set; }

    public PosRegister? Register { get; set; }

    public PosShiftStatus Status { get; set; } = PosShiftStatus.Open;

    public required string OpenedBy { get; set; }

    public DateTime OpenedAtUtc { get; set; }

    public decimal OpeningFloat { get; set; }

    public string? ClosedBy { get; set; }

    public DateTime? ClosedAtUtc { get; set; }

    public decimal? ExpectedCash { get; set; }

    public decimal? CountedCash { get; set; }

    public ICollection<PosCashMovement> CashMovements { get; set; } = [];
}

public class PosCashMovement : BaseEntity
{
    public Guid ShiftId { get; set; }

    public PosShift? Shift { get; set; }

    public PosCashMovementType Type { get; set; }

    public decimal Amount { get; set; }

    public required string Reason { get; set; }

    public required string User { get; set; }
}

public class PaymentTerminal : BaseEntity
{
    public Guid? RegisterId { get; set; }

    public PosRegister? Register { get; set; }

    /// <summary>Key of the registered <c>IPaymentTerminalProvider</c> (e.g. "mock").</summary>
    public required string Provider { get; set; }

    /// <summary>Identifier of the device at the provider (serial / terminal id).</summary>
    public required string ProviderTerminalId { get; set; }

    public required string DisplayName { get; set; }

    public bool IsActive { get; set; } = true;
}

public class SalePayment : BaseEntity
{
    public Guid SaleId { get; set; }

    public Sale? Sale { get; set; }

    public SalePaymentMethod Method { get; set; }

    public SalePaymentStatus Status { get; set; }

    /// <summary>Amount applied to the sale (cash excludes change given back).</summary>
    public decimal Amount { get; set; }

    public decimal? TenderedAmount { get; set; }

    public decimal? ChangeGiven { get; set; }

    public ICollection<PaymentAttempt> Attempts { get; set; } = [];
}

/// <summary>One interaction with a card terminal. Only brand and last four digits are ever stored.</summary>
public class PaymentAttempt : BaseEntity
{
    public Guid SalePaymentId { get; set; }

    public SalePayment? SalePayment { get; set; }

    public Guid TerminalId { get; set; }

    public PaymentTerminal? Terminal { get; set; }

    public required string Provider { get; set; }

    public required string IdempotencyKey { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "EUR";

    public PaymentAttemptStatus Status { get; set; }

    public string? ProviderReference { get; set; }

    public string? AuthorizationCode { get; set; }

    public string? CardBrand { get; set; }

    public string? CardLast4 { get; set; }

    public string? FailureReason { get; set; }

    public string? CustomerReceiptText { get; set; }

    public string? MerchantReceiptText { get; set; }

    public DateTime RequestedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }
}
