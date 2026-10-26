using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Pos;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Services.Pos;

/// <summary>
/// Simulated card terminal for development and tests. The amount's cents pick the outcome:
/// <c>.01</c> declined, <c>.02</c> never answers (stays pending until cancelled), anything else approved
/// after <see cref="PosPaymentOptions.MockApprovalDelay"/>.
/// </summary>
public sealed class MockPaymentTerminalProvider(IOptions<PosPaymentOptions> options, TimeProvider timeProvider)
    : IPaymentTerminalProvider
{
    public const string ProviderKey = "mock";

    private readonly ConcurrentDictionary<string, Operation> _operations = new();
    private readonly ConcurrentDictionary<string, string> _byIdempotencyKey = new();

    public string Key => ProviderKey;

    public Task<TerminalPaymentResult> StartPaymentAsync(TerminalPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var reference = _byIdempotencyKey.GetOrAdd(request.IdempotencyKey, _ =>
        {
            var created = $"MOCK-{Guid.NewGuid():N}";
            _operations[created] = new Operation(request.Amount, timeProvider.GetUtcNow());
            return created;
        });

        return Task.FromResult(Evaluate(reference));
    }

    public Task<TerminalPaymentResult> GetPaymentStatusAsync(string providerReference, CancellationToken cancellationToken = default)
        => Task.FromResult(Evaluate(providerReference));

    public Task<TerminalPaymentResult> CancelPaymentAsync(string providerReference, CancellationToken cancellationToken = default)
    {
        if (!_operations.TryGetValue(providerReference, out var operation))
        {
            return Task.FromResult(new TerminalPaymentResult(PaymentAttemptStatus.Unknown, providerReference, FailureReason: "Unknown reference"));
        }

        // A payment already approved cannot be cancelled; it has to be refunded.
        if (Evaluate(providerReference).Status == PaymentAttemptStatus.Pending)
        {
            operation.Cancelled = true;
        }

        return Task.FromResult(Evaluate(providerReference));
    }

    public Task<TerminalPaymentResult> RefundAsync(TerminalRefundRequest request, CancellationToken cancellationToken = default)
    {
        var reference = $"MOCK-REFUND-{Guid.NewGuid():N}";
        return Task.FromResult(new TerminalPaymentResult(
            PaymentAttemptStatus.Approved,
            reference,
            AuthorizationCode: AuthCode(reference),
            CardBrand: "VISA",
            CardLast4: "4242",
            CustomerReceiptText: $"DEVOLUCION {request.Amount:0.00} {request.Currency}"));
    }

    public TerminalWebhookEvent? ParseWebhook(string body, IReadOnlyDictionary<string, string> headers)
    {
        if (!headers.TryGetValue("X-Mock-Signature", out var signature))
        {
            return null;
        }

        var expected = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(options.Value.MockWebhookSecret),
            Encoding.UTF8.GetBytes(body)));

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(signature.ToUpperInvariant())))
        {
            return null;
        }

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var reference = root.GetProperty("providerReference").GetString();
        if (string.IsNullOrWhiteSpace(reference) || !Enum.TryParse<PaymentAttemptStatus>(root.GetProperty("status").GetString(), true, out var status))
        {
            return null;
        }

        return new TerminalWebhookEvent(reference, new TerminalPaymentResult(
            status,
            reference,
            Read(root, "authorizationCode"),
            Read(root, "cardBrand"),
            Read(root, "cardLast4"),
            Read(root, "failureReason")));
    }

    private TerminalPaymentResult Evaluate(string reference)
    {
        if (!_operations.TryGetValue(reference, out var operation))
        {
            return new TerminalPaymentResult(PaymentAttemptStatus.Unknown, reference, FailureReason: "Unknown reference");
        }

        if (operation.Cancelled)
        {
            return new TerminalPaymentResult(PaymentAttemptStatus.Cancelled, reference, FailureReason: "Cancelled at terminal");
        }

        var cents = (int)(Math.Abs(operation.Amount) * 100m % 100m);
        if (cents == 2 || timeProvider.GetUtcNow() - operation.StartedAt < options.Value.MockApprovalDelay)
        {
            return new TerminalPaymentResult(PaymentAttemptStatus.Pending, reference);
        }

        if (cents == 1)
        {
            return new TerminalPaymentResult(PaymentAttemptStatus.Declined, reference, FailureReason: "Declined by issuer");
        }

        return new TerminalPaymentResult(
            PaymentAttemptStatus.Approved,
            reference,
            AuthorizationCode: AuthCode(reference),
            CardBrand: "VISA",
            CardLast4: "4242",
            CustomerReceiptText: $"VISA ****4242  {operation.Amount:0.00} EUR  AUT {AuthCode(reference)}",
            MerchantReceiptText: $"VISA ****4242  {operation.Amount:0.00} EUR  AUT {AuthCode(reference)}  REF {reference}");
    }

    private static string AuthCode(string reference)
        => (Math.Abs(reference.GetHashCode()) % 1_000_000).ToString("D6");

    private static string? Read(JsonElement root, string name)
        => root.TryGetProperty(name, out var value) ? value.GetString() : null;

    private sealed class Operation(decimal amount, DateTimeOffset startedAt)
    {
        public decimal Amount { get; } = amount;

        public DateTimeOffset StartedAt { get; } = startedAt;

        public bool Cancelled { get; set; }
    }
}
