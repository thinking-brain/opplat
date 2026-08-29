using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Opplat.Infrastructure.Services.Billing;

public sealed class MockPaymentGatewayService(
    AdminTenantCatalogDbContext dbContext,
    IOptions<OpplatLegalDataOptions> legalDataOptions,
    ILogger<MockPaymentGatewayService> logger) : IPaymentGatewayService
{
    private readonly ILogger<MockPaymentGatewayService> _logger = logger;
    private readonly OpplatLegalDataOptions _legalDataOptions = legalDataOptions.Value;

    static MockPaymentGatewayService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

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

        var tenantName = invoice.Tenant?.Name ?? "Opplat Tenant";
        var tenantIdentifier = invoice.Tenant?.Identifier ?? "tenant";
        var periodLabel = $"{invoice.PeriodStart:dd MMM yyyy} – {invoice.PeriodEnd:dd MMM yyyy}";
        var invoiceDate = invoice.PaidAt?.ToString("dd MMM yyyy") ?? invoice.PeriodEnd.ToString("dd MMM yyyy");
        var opplatCompanyName = string.IsNullOrWhiteSpace(_legalDataOptions.CompanyName) ? "Opplat" : _legalDataOptions.CompanyName;
        var opplatTaxId = string.IsNullOrWhiteSpace(_legalDataOptions.TaxId) ? "" : _legalDataOptions.TaxId;
        var opplatAddress = string.IsNullOrWhiteSpace(_legalDataOptions.Address) ? "" : _legalDataOptions.Address;
        var opplatEmail = string.IsNullOrWhiteSpace(_legalDataOptions.Email) ? "hello@opplat.example" : _legalDataOptions.Email;
        var opplatPhone = string.IsNullOrWhiteSpace(_legalDataOptions.Phone) ? "" : _legalDataOptions.Phone;

        static string FormatCurrency(decimal amount, string currency) =>
            $"{amount:0.00} {currency}";

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(style => style.FontSize(10));

                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(opplatCompanyName.ToUpperInvariant()).Bold().FontSize(20);
                            c.Item().Text("Subscription Invoice").FontSize(11);
                            c.Item().Text("B2B billing document").FontSize(9).FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(180).Column(c =>
                        {
                            c.Item().AlignRight().Text("Invoice").Bold().FontSize(16);
                            c.Item().AlignRight().Text(invoiceId).FontSize(11);
                            c.Item().AlignRight().Text($"Issued {invoiceDate}").FontSize(9);
                        });
                    });

                    col.Item().PaddingVertical(16).LineHorizontal(0.5f);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Billed to").Bold();
                            c.Item().Text(tenantName);
                            c.Item().Text($"Tenant: {tenantIdentifier}");
                            c.Item().Text($"Billing period: {periodLabel}");
                        });

                        row.ConstantItem(180).Column(c =>
                        {
                            c.Item().Text("Company details").Bold();
                            c.Item().Text(opplatCompanyName);
                            if (!string.IsNullOrWhiteSpace(opplatTaxId)) c.Item().Text($"Tax ID: {opplatTaxId}");
                            if (!string.IsNullOrWhiteSpace(opplatAddress)) c.Item().Text(opplatAddress);
                            if (!string.IsNullOrWhiteSpace(opplatEmail)) c.Item().Text(opplatEmail);
                            if (!string.IsNullOrWhiteSpace(opplatPhone)) c.Item().Text(opplatPhone);
                        });
                    });

                    col.Item().PaddingVertical(16).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Description").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignCenter().Text("Qty").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Amount").Bold();
                        });

                        table.Cell().Padding(4).Text($"Subscription plan for {tenantName}");
                        table.Cell().Padding(4).AlignCenter().Text("1");
                        table.Cell().Padding(4).AlignRight().Text(FormatCurrency(invoice.AmountDue, invoice.Currency));
                    });

                    col.Item().PaddingTop(14).LineHorizontal(0.5f);

                    col.Item().AlignRight().Column(c =>
                    {
                        c.Item().Text($"Subtotal: {FormatCurrency(invoice.AmountDue, invoice.Currency)}");
                        c.Item().Text($"Tax: {FormatCurrency(0m, invoice.Currency)}");
                        c.Item().Text($"Total due: {FormatCurrency(invoice.AmountDue, invoice.Currency)}").Bold().FontSize(12);
                    });

                    col.Item().PaddingTop(16).Text("This invoice was generated by Opplat for the active subscription period.").FontSize(9).FontColor(Colors.Grey.Darken2);
                });
            });
        }).GeneratePdf();

        return pdfBytes;
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
