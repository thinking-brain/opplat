using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Features.Sales.Pos;

public enum PosErrorKind
{
    None,
    NotFound,
    Validation,
    Conflict
}

public sealed record PosResult<T>(bool Succeeded, string Message, PosErrorKind Error, T? Data)
{
    public static PosResult<T> Ok(T data, string message = "OK") => new(true, message, PosErrorKind.None, data);

    public static PosResult<T> Fail(PosErrorKind error, string message) => new(false, message, error, default);

    public static PosResult<T> NotFound(string message = "Entity not found.") => Fail(PosErrorKind.NotFound, message);

    public static PosResult<T> Invalid(string message) => Fail(PosErrorKind.Validation, message);

    public static PosResult<T> Conflict(string message) => Fail(PosErrorKind.Conflict, message);
}

public sealed record RegisterDto(Guid Id, string Code, string Name, string InvoiceSeries, bool IsActive, Guid? StorageId);

public sealed record TerminalDto(Guid Id, Guid? RegisterId, string Provider, string ProviderTerminalId, string DisplayName, bool IsActive);

public sealed record ShiftDto(
    Guid Id,
    Guid RegisterId,
    PosShiftStatus Status,
    string OpenedBy,
    DateTime OpenedAtUtc,
    decimal OpeningFloat,
    DateTime? ClosedAtUtc,
    decimal? ExpectedCash,
    decimal? CountedCash);

public sealed record ReportLineDto(string Key, int Count, decimal Amount);

public sealed record TaxReportLineDto(decimal Rate, decimal TaxableBase, decimal TaxAmount);

/// <summary>Z-report: what the shift sold, how it was paid, and the cash that should be in the drawer.</summary>
public sealed record ShiftReportDto(
    ShiftDto Shift,
    int SalesCount,
    decimal TotalSales,
    IReadOnlyList<ReportLineDto> ByPaymentMethod,
    IReadOnlyList<TaxReportLineDto> ByTaxRate,
    decimal PayIn,
    decimal PayOut,
    decimal ExpectedCash);

public sealed record SaleLineDto(Guid ProductId, string Name, int Quantity, decimal UnitPrice, decimal TaxRate, decimal Discount, decimal Amount);

public sealed record AttemptDto(
    Guid Id,
    PaymentAttemptStatus Status,
    string Provider,
    string? ProviderReference,
    string? AuthorizationCode,
    string? CardBrand,
    string? CardLast4,
    string? FailureReason,
    string? CustomerReceiptText);

public sealed record PaymentDto(
    Guid Id,
    Guid SaleId,
    SalePaymentMethod Method,
    SalePaymentStatus Status,
    decimal Amount,
    decimal? TenderedAmount,
    decimal? ChangeGiven,
    AttemptDto? Attempt);

public sealed record SaleDto(
    Guid Id,
    SaleStatus Status,
    DateTime Date,
    Guid? RegisterId,
    Guid? ShiftId,
    string? Cashier,
    string Currency,
    decimal Total,
    decimal Paid,
    decimal Due,
    IReadOnlyList<SaleLineDto> Lines,
    IReadOnlyList<PaymentDto> Payments,
    Guid? InvoiceId,
    string? InvoiceNumber);

public sealed record SaleLineRequest(Guid ProductId, int Quantity, decimal Discount = 0m);

public sealed record CreateSaleRequest(Guid RegisterId, IReadOnlyList<SaleLineRequest> Lines);

public sealed record AddPaymentRequest(
    SalePaymentMethod Method,
    decimal? Amount,
    Guid? TerminalId,
    string? IdempotencyKey);

public sealed record CustomerRequest(string Name, string? TaxId, string? Address, string? Country);

public sealed record CompleteSaleRequest(CustomerRequest? Customer);
