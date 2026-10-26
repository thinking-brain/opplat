using Opplat.Application.Abstractions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Opplat.Infrastructure.Services.Billing;

/// <summary>
/// Renders the subscription-invoice PDF shared by every <c>IPaymentGatewayService</c>
/// implementation (Mock, Redsys, ...), since none of them proxy a gateway-hosted invoice PDF.
/// </summary>
public static class SubscriptionInvoicePdfRenderer
{
    static SubscriptionInvoicePdfRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Render(
        string invoiceId,
        string tenantName,
        string tenantIdentifier,
        DateTime periodStart,
        DateTime periodEnd,
        DateTime invoiceDate,
        decimal amountDue,
        string currency,
        OpplatLegalDataOptions legalData)
    {
        var periodLabel = $"{periodStart:dd MMM yyyy} – {periodEnd:dd MMM yyyy}";
        var opplatCompanyName = string.IsNullOrWhiteSpace(legalData.CompanyName) ? "Opplat" : legalData.CompanyName;
        var opplatTaxId = string.IsNullOrWhiteSpace(legalData.TaxId) ? "" : legalData.TaxId;
        var opplatAddress = string.IsNullOrWhiteSpace(legalData.Address) ? "" : legalData.Address;
        var opplatEmail = string.IsNullOrWhiteSpace(legalData.Email) ? "hello@opplat.example" : legalData.Email;
        var opplatPhone = string.IsNullOrWhiteSpace(legalData.Phone) ? "" : legalData.Phone;

        static string FormatCurrency(decimal amount, string currency) =>
            $"{amount:0.00} {currency}";

        return Document.Create(container =>
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
                            c.Item().AlignRight().Text($"Issued {invoiceDate:dd MMM yyyy}").FontSize(9);
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
                        table.Cell().Padding(4).AlignRight().Text(FormatCurrency(amountDue, currency));
                    });

                    col.Item().PaddingTop(14).LineHorizontal(0.5f);

                    col.Item().AlignRight().Column(c =>
                    {
                        c.Item().Text($"Subtotal: {FormatCurrency(amountDue, currency)}");
                        c.Item().Text($"Tax: {FormatCurrency(0m, currency)}");
                        c.Item().Text($"Total due: {FormatCurrency(amountDue, currency)}").Bold().FontSize(12);
                    });

                    col.Item().PaddingTop(16).Text("This invoice was generated by Opplat for the active subscription period.").FontSize(9).FontColor(Colors.Grey.Darken2);
                });
            });
        }).GeneratePdf();
    }
}
