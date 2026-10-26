using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using QRCoder;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Builds the AEAT invoice-lookup QR payload and renders it (ISO/IEC 18004:2015, error correction level M).
/// </summary>
/// <remarks>
/// The printed QR must measure 30–40 mm per side and be accompanied by the legend
/// "VERI*FACTU" (Verifactu mode) or "Factura verificable en la sede electrónica de la AEAT".
/// Parameter names and URL are from the AEAT QR specification; confirm against its PDF.
/// </remarks>
public static class VerifactuQrCodeBuilder
{
    public const string ProductionUrl = "https://www2.agenciatributaria.gob.es/wlpl/TIKE-CONT/ValidarQR";
    public const string TestUrl = "https://prewww2.aeat.es/wlpl/TIKE-CONT/ValidarQR";

    /// <summary>Builds the lookup URL from the issuer NIF and the invoice identity fields.</summary>
    public static string BuildPayload(string issuerNif, Invoice invoice, bool useTestEnvironment = false)
        => BuildPayload(
            issuerNif,
            invoice.FullNumber,
            FiscalTime.ToFiscalDate(invoice.IssueDate),
            invoice.TotalAmount,
            useTestEnvironment);

    public static string BuildPayload(
        string issuerNif,
        string invoiceNumber,
        DateOnly issueDate,
        decimal totalAmount,
        bool useTestEnvironment = false)
        => $"{(useTestEnvironment ? TestUrl : ProductionUrl)}?nif={Uri.EscapeDataString(issuerNif.Trim())}" +
           $"&numserie={Uri.EscapeDataString(invoiceNumber.Trim())}" +
           $"&fecha={VerifactuHashCalculator.FormatDate(issueDate)}" +
           $"&importe={VerifactuHashCalculator.FormatAmount(totalAmount)}";

    /// <summary>Renders a QR code PNG byte array for the given <paramref name="payload"/>.</summary>
    public static byte[] RenderPng(string payload, int pixelsPerModule = 5)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(data);
        return qrCode.GetGraphic(pixelsPerModule);
    }
}
