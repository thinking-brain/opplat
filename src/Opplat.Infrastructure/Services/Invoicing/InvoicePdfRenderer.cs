using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Opplat.Infrastructure.Services.Invoicing;

/// <summary>
/// Generates invoice PDFs using QuestPDF (Community license).
/// Simplified invoices use a compact ticket layout; full invoices include issuer /
/// recipient blocks, line items, a tax breakdown table, and the Verifactu QR code.
/// </summary>
public sealed class InvoicePdfRenderer : IInvoicePdfRenderer
{
    static InvoicePdfRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Render(Invoice invoice)
    {
        return invoice.InvoiceType == InvoiceType.Simplified
            ? RenderSimplified(invoice)
            : RenderFull(invoice);
    }

    // ── Simplified / ticket layout ───────────────────────────────────────────
    private static byte[] RenderSimplified(Invoice invoice)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(15);
                page.Content().Column(col =>
                {
                    col.Item().AlignCenter().Text("FACTURA SIMPLIFICADA").Bold().FontSize(14);
                    col.Item().AlignCenter().Text(invoice.FullNumber).FontSize(11);
                    col.Item().PaddingVertical(4).LineHorizontal(0.5f);

                    col.Item().Text($"Fecha: {invoice.IssueDate:dd/MM/yyyy}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(invoice.PaymentMethod))
                        col.Item().Text($"Pago: {invoice.PaymentMethod}").FontSize(9);

                    col.Item().PaddingTop(6).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(5);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Concepto").Bold().FontSize(8);
                            header.Cell().AlignCenter().Text("Cant.").Bold().FontSize(8);
                            header.Cell().AlignRight().Text("Total").Bold().FontSize(8);
                        });

                        foreach (var line in invoice.Lines)
                        {
                            table.Cell().Text(line.Description).FontSize(8);
                            table.Cell().AlignCenter().Text(line.Quantity.ToString("G")).FontSize(8);
                            table.Cell().AlignRight().Text(line.LineTotal.ToString("0.00 €")).FontSize(8);
                        }
                    });

                    col.Item().PaddingTop(4).LineHorizontal(0.5f);
                    col.Item().AlignRight().Text($"TOTAL: {invoice.TotalAmount:0.00} €").Bold().FontSize(11);

                    AddVerifactuBlock(col, invoice);
                });
            });
        }).GeneratePdf();
    }

    // ── Full invoice layout ───────────────────────────────────────────────────
    private static byte[] RenderFull(Invoice invoice)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("FACTURA").Bold().FontSize(20);
                            c.Item().Text(invoice.FullNumber).FontSize(13);
                            c.Item().Text($"Fecha: {invoice.IssueDate:dd/MM/yyyy}").FontSize(10);
                        });
                        row.ConstantItem(120).Column(c =>
                        {
                            c.Item().AlignRight().Text($"Núm.: {invoice.FullNumber}").Bold().FontSize(10);
                            c.Item().AlignRight().Text($"Serie: {invoice.Series}").FontSize(10);
                        });
                    });

                    col.Item().PaddingVertical(8).LineHorizontal(0.5f);

                    // Recipient block
                    if (!string.IsNullOrWhiteSpace(invoice.CustomerSnapshot?.Name))
                    {
                        col.Item().Column(c =>
                        {
                            c.Item().Text("DESTINATARIO").Bold().FontSize(9);
                            c.Item().Text(invoice.CustomerSnapshot.Name).FontSize(10);
                            if (!string.IsNullOrWhiteSpace(invoice.CustomerSnapshot.TaxId))
                                c.Item().Text($"NIF/CIF: {invoice.CustomerSnapshot.TaxId}").FontSize(9);
                            if (!string.IsNullOrWhiteSpace(invoice.CustomerSnapshot.Address))
                                c.Item().Text(invoice.CustomerSnapshot.Address).FontSize(9);
                        });
                        col.Item().PaddingVertical(6).LineHorizontal(0.5f);
                    }

                    // Line items
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(5);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            foreach (var h in new[] { "Descripción", "Cant.", "P. Unitario", "IVA %", "Total" })
                                header.Cell().Background(Colors.Grey.Lighten3).Text(h).Bold().FontSize(9);
                        });

                        foreach (var line in invoice.Lines)
                        {
                            table.Cell().Text(line.Description).FontSize(9);
                            table.Cell().AlignCenter().Text(line.Quantity.ToString("G")).FontSize(9);
                            table.Cell().AlignRight().Text(line.UnitPrice.ToString("0.00 €")).FontSize(9);
                            table.Cell().AlignCenter().Text($"{line.TaxRate}%").FontSize(9);
                            table.Cell().AlignRight().Text(line.LineTotal.ToString("0.00 €")).FontSize(9);
                        }
                    });

                    col.Item().PaddingTop(6).LineHorizontal(0.5f);

                    // Tax breakdown + totals
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            if (invoice.TaxBreakdowns.Any())
                            {
                                c.Item().Text("Desglose de impuestos").Bold().FontSize(9);
                                foreach (var tb in invoice.TaxBreakdowns)
                                    c.Item().Text(
                                        $"{tb.TaxType} {tb.Rate}%: base {tb.TaxableBase:0.00} € → impuesto {tb.TaxAmount:0.00} €")
                                        .FontSize(9);
                            }
                        });
                        row.ConstantItem(160).Column(c =>
                        {
                            c.Item().AlignRight().Text($"Subtotal: {invoice.Subtotal:0.00} €").FontSize(10);
                            var tax = invoice.TaxBreakdowns.Sum(t => t.TaxAmount);
                            c.Item().AlignRight().Text($"IVA: {tax:0.00} €").FontSize(10);
                            c.Item().AlignRight().Text($"TOTAL: {invoice.TotalAmount:0.00} €").Bold().FontSize(13);
                        });
                    });

                    if (!string.IsNullOrWhiteSpace(invoice.Notes))
                    {
                        col.Item().PaddingTop(8).Text($"Notas: {invoice.Notes}").FontSize(9).Italic();
                    }

                    AddVerifactuBlock(col, invoice);
                });
            });
        }).GeneratePdf();
    }

    // ── Shared: Verifactu legend + QR ────────────────────────────────────────
    private static void AddVerifactuBlock(ColumnDescriptor col, Invoice invoice)
    {
        var record = invoice.FiscalRecord;
        if (record is null) return;

        col.Item().PaddingTop(10).LineHorizontal(0.5f);

        if (record.SubmissionMode == FiscalSubmissionMode.Verifactu)
        {
            col.Item().PaddingTop(4).AlignCenter().Text("VERI*FACTU").Bold().FontSize(8);
        }
        else if (record.SubmissionMode == FiscalSubmissionMode.NonVerifactuSigned)
        {
            col.Item().PaddingTop(4).AlignCenter()
                .Text("Factura verificable en la sede electrónica de la AEAT").FontSize(7).Italic();
        }

        if (!string.IsNullOrWhiteSpace(record.QrCodePayload))
        {
            try
            {
                var qrPng = VerifactuQrCodeBuilder.RenderPng(record.QrCodePayload);
                col.Item().AlignCenter().Width(60).Image(qrPng);
            }
            catch
            {
                // QR rendering is non-critical; omit if it fails
            }
        }
    }
}
