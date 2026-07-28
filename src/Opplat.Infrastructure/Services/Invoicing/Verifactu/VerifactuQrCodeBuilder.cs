using Opplat.Domain.Entities.Invoicing;
using QRCoder;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Builds the AEAT Verifactu QR verification payload and renders a QR code PNG.
/// </summary>
/// <remarks>
/// <b>TODO (compliance verification required):</b> The exact verification URL, query
/// parameter names, date format, and amount format MUST be verified against the official
/// AEAT PDF "Características del código QR y especificaciones del servicio de cotejo de
/// la factura" from <c>sede.agenciatributaria.gob.es</c> before enabling live AEAT submission.
/// </remarks>
public static class VerifactuQrCodeBuilder
{
    // TODO: verify the correct AEAT QR verification base URL against the official spec PDF.
    private const string VerificationBaseUrl =
        "https://www2.agenciatributaria.gob.es/wlpl/TIKE-CONT/ValidarQR";

    /// <summary>
    /// Builds the AEAT cotejo URL that encodes the invoice identity fields.
    /// This string is stored in <see cref="InvoiceFiscalRecord.QrCodePayload"/> and
    /// encoded into the printed QR image via <see cref="RenderPng"/>.
    /// </summary>
    public static string BuildPayload(string issuerNif, Invoice invoice, InvoiceFiscalRecord record)
    {
        // TODO: verify exact parameter names (nif / numserie / fecha / importe) and
        // date/amount formats against the official AEAT QR spec PDF.
        var date = invoice.IssueDate.ToString("dd-MM-yyyy");
        var amount = invoice.TotalAmount.ToString("0.00");
        return $"{VerificationBaseUrl}?nif={Uri.EscapeDataString(issuerNif)}" +
               $"&numserie={Uri.EscapeDataString(invoice.FullNumber)}" +
               $"&fecha={Uri.EscapeDataString(date)}" +
               $"&importe={Uri.EscapeDataString(amount)}";
    }

    /// <summary>Renders a QR code PNG byte array for the given <paramref name="payload"/>.</summary>
    public static byte[] RenderPng(string payload, int pixelsPerModule = 5)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(data);
        return qrCode.GetGraphic(pixelsPerModule);
    }
}
