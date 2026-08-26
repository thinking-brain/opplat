using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Services.Billing;

public sealed class NoOpPaymentGatewayService(ILogger<NoOpPaymentGatewayService> logger) : IPaymentGatewayService
{
    private readonly ILogger<NoOpPaymentGatewayService> _logger = logger;

    public Task<PaymentGatewayResult> CreateCustomerAsync(string email, string businessName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Payment gateway disabled; skipped customer creation for {Email}.", email);
        return Task.FromResult(new PaymentGatewayResult(true, $"local_customer_{Guid.NewGuid():N}"));
    }

    public Task<PaymentGatewayResult> AttachPaymentMethodAsync(string customerId, string paymentMethodId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, paymentMethodId));

    public Task<PaymentGatewaySubscriptionResult> CreateSubscriptionAsync(string customerId, string priceId, BillingInterval interval, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewaySubscriptionResult(
            true,
            $"local_subscription_{Guid.NewGuid():N}",
            TenantBillingStatus.Active,
            DateTime.UtcNow.Add(interval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30))));

    public Task<PaymentGatewayResult> CancelSubscriptionAsync(string subscriptionId, bool atPeriodEnd, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayResult> ChangeSubscriptionPriceAsync(string subscriptionId, string priceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayPortalResult> CreateBillingPortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayPortalResult(true, returnUrl));
}
