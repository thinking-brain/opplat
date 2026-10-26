using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services.Billing;

public sealed class MockPaymentGatewayService(
    AdminTenantCatalogDbContext dbContext,
    IOptions<OpplatLegalDataOptions> legalDataOptions,
    ILogger<MockPaymentGatewayService> logger) : IPaymentGatewayService
{
    private readonly ILogger<MockPaymentGatewayService> _logger = logger;
    private readonly OpplatLegalDataOptions _legalDataOptions = legalDataOptions.Value;

    public Task<PaymentGatewayCardResult> CreatePaymentMethodAsync(
        string cardNumber,
        int expMonth,
        int expYear,
        string cvc,
        CancellationToken cancellationToken = default)
    {
        var digits = new string(cardNumber.Where(char.IsDigit).ToArray());
        if (digits.Length is < 12 or > 19 || !IsValidLuhn(digits))
            return Task.FromResult(new PaymentGatewayCardResult(false, Error: "The card number is invalid."));
        if (expMonth is < 1 or > 12 || expYear < DateTime.UtcNow.Year || (expYear == DateTime.UtcNow.Year && expMonth < DateTime.UtcNow.Month))
            return Task.FromResult(new PaymentGatewayCardResult(false, Error: "The card expiry date is invalid."));
        if (cvc.Length is < 3 or > 4 || !cvc.All(char.IsDigit))
            return Task.FromResult(new PaymentGatewayCardResult(false, Error: "The card security code is invalid."));

        var brand = digits.StartsWith('4') ? "visa" : digits.StartsWith("34") || digits.StartsWith("37") ? "amex" : "mastercard";
        return Task.FromResult(new PaymentGatewayCardResult(true, $"local_pm_{Guid.NewGuid():N}", brand, digits[^4..]));
    }

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
            DateTime.UtcNow.Add(interval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30)),
            $"local_invoice_{Guid.NewGuid():N}"));

    public Task<PaymentGatewayResult> CancelSubscriptionAsync(string subscriptionId, bool atPeriodEnd, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayResult> ChangeSubscriptionPriceAsync(string subscriptionId, string priceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayResult(true, subscriptionId));

    public Task<PaymentGatewayPortalResult> CreateBillingPortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewayPortalResult(true, returnUrl));

    public Task<PaymentGatewaySubscriptionResult> RenewSubscriptionAsync(string subscriptionId, BillingInterval interval, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PaymentGatewaySubscriptionResult(
            true,
            subscriptionId,
            TenantBillingStatus.Active,
            DateTime.UtcNow.Add(interval == BillingInterval.Annual ? TimeSpan.FromDays(365) : TimeSpan.FromDays(30)),
            $"local_invoice_{Guid.NewGuid():N}"));

    public async Task<byte[]?> DownloadInvoicePdfAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await dbContext.SubscriptionInvoices
            .AsNoTracking()
            .Include(item => item.Tenant)
            .FirstOrDefaultAsync(item => item.StripeInvoiceId == invoiceId, cancellationToken);

        if (invoice is null)
            return null;

        return SubscriptionInvoicePdfRenderer.Render(
            invoiceId,
            invoice.Tenant?.Name ?? "Opplat Tenant",
            invoice.Tenant?.Identifier ?? "tenant",
            invoice.PeriodStart,
            invoice.PeriodEnd,
            invoice.PaidAt ?? invoice.PeriodEnd,
            invoice.AmountDue,
            invoice.Currency,
            _legalDataOptions);
    }

    private static bool IsValidLuhn(string digits)
    {
        var sum = 0;
        var doubleDigit = false;
        for (var index = digits.Length - 1; index >= 0; index--)
        {
            var value = digits[index] - '0';
            if (doubleDigit && (value *= 2) > 9)
                value -= 9;
            sum += value;
            doubleDigit = !doubleDigit;
        }
        return sum % 10 == 0;
    }
}
