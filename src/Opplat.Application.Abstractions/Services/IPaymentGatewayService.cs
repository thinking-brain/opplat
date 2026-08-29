using Opplat.Domain.Entities.Administration;

namespace Opplat.Application.Abstractions.Services;

public interface IPaymentGatewayService
{
    Task<PaymentGatewayCardResult> CreatePaymentMethodAsync(
        string cardNumber,
        int expMonth,
        int expYear,
        string cvc,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayResult> CreateCustomerAsync(
        string email,
        string businessName,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayResult> AttachPaymentMethodAsync(
        string customerId,
        string paymentMethodId,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewaySubscriptionResult> CreateSubscriptionAsync(
        string customerId,
        string priceId,
        BillingInterval interval,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayResult> CancelSubscriptionAsync(
        string subscriptionId,
        bool atPeriodEnd,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayResult> ChangeSubscriptionPriceAsync(
        string subscriptionId,
        string priceId,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayPortalResult> CreateBillingPortalSessionAsync(
        string customerId,
        string returnUrl,
        CancellationToken cancellationToken = default);

    Task<byte[]?> DownloadInvoicePdfAsync(
        string invoiceId,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewaySubscriptionResult> RenewSubscriptionAsync(
        string subscriptionId,
        BillingInterval interval,
        CancellationToken cancellationToken = default);
}

public sealed record PaymentGatewayResult(
    bool Succeeded,
    string? ExternalId = null,
    string? Error = null);

public sealed record PaymentGatewayCardResult(
    bool Succeeded,
    string? ExternalId = null,
    string? Brand = null,
    string? Last4 = null,
    string? Error = null);

public sealed record PaymentGatewaySubscriptionResult(
    bool Succeeded,
    string? SubscriptionId = null,
    TenantBillingStatus BillingStatus = TenantBillingStatus.Active,
    DateTime? CurrentPeriodEnd = null,
    string? InvoiceId = null,
    string? Error = null);

public sealed record PaymentGatewayPortalResult(
    bool Succeeded,
    string? Url = null,
    string? Error = null);
