using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Abstractions.Pos;

public sealed record TerminalPaymentRequest(
    string IdempotencyKey,
    string ProviderTerminalId,
    decimal Amount,
    string Currency,
    string Reference);

public sealed record TerminalRefundRequest(
    string IdempotencyKey,
    string ProviderTerminalId,
    string OriginalProviderReference,
    decimal Amount,
    string Currency);

/// <summary>State of a card-terminal operation. Never contains a full card number.</summary>
public sealed record TerminalPaymentResult(
    PaymentAttemptStatus Status,
    string? ProviderReference = null,
    string? AuthorizationCode = null,
    string? CardBrand = null,
    string? CardLast4 = null,
    string? FailureReason = null,
    string? CustomerReceiptText = null,
    string? MerchantReceiptText = null);

public sealed record TerminalWebhookEvent(string ProviderReference, TerminalPaymentResult Result);

/// <summary>
/// Vendor-neutral contract for a physical card terminal. Operations are asynchronous:
/// <see cref="StartPaymentAsync"/> normally returns <see cref="PaymentAttemptStatus.Pending"/>
/// and the final result arrives through <see cref="GetPaymentStatusAsync"/> or a webhook.
/// Implementations must be idempotent on <see cref="TerminalPaymentRequest.IdempotencyKey"/>.
/// </summary>
public interface IPaymentTerminalProvider
{
    string Key { get; }

    Task<TerminalPaymentResult> StartPaymentAsync(TerminalPaymentRequest request, CancellationToken cancellationToken = default);

    Task<TerminalPaymentResult> GetPaymentStatusAsync(string providerReference, CancellationToken cancellationToken = default);

    Task<TerminalPaymentResult> CancelPaymentAsync(string providerReference, CancellationToken cancellationToken = default);

    Task<TerminalPaymentResult> RefundAsync(TerminalRefundRequest request, CancellationToken cancellationToken = default);

    /// <summary>Verifies the signature and parses a provider callback; returns null when it is not authentic.</summary>
    TerminalWebhookEvent? ParseWebhook(string body, IReadOnlyDictionary<string, string> headers);
}

public interface IPaymentTerminalProviderRegistry
{
    IReadOnlyCollection<string> Keys { get; }

    IPaymentTerminalProvider? Find(string key);
}
