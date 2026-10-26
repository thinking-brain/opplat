using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Computes the SHA-256 "huella" of a billing record as defined by AEAT
/// ("Algoritmo de cálculo de codificación de la huella o hash", RD 1007/2023 / Orden HAC/1177/2024):
/// <c>campo=valor&amp;campo=valor…</c> in a fixed field order, UTF-8, hex digest in upper case.
/// </summary>
/// <remarks>
/// The XML record must serialise numbers and dates with exactly the same text used here
/// (see <see cref="FormatAmount"/> and <see cref="FormatDate"/>). Verify against the worked
/// examples in the AEAT PDF; <c>VerifactuHashCalculatorTests</c> holds the vectors.
/// </remarks>
public static class VerifactuHashCalculator
{
    public static string FormatAmount(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);

    public static string FormatDate(DateOnly date) => date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

    public static string FormatTimestamp(DateTimeOffset value)
        => new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Offset)
            .ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture);

    public static string InvoiceTypeCode(InvoiceType type) => type switch
    {
        InvoiceType.Simplified => "F2",
        InvoiceType.Full => "F1",
        InvoiceType.Substitutive => "F3",
        InvoiceType.Rectifying => "R1",
        _ => "F1"
    };

    /// <summary>Hash input and digest for a RegistroAlta.</summary>
    public static (string Input, string Hash) ComputeAlta(
        string issuerNif,
        string invoiceNumber,
        DateOnly issueDate,
        string invoiceTypeCode,
        decimal totalTax,
        decimal totalAmount,
        string? previousHash,
        DateTimeOffset generatedAt)
    {
        var input =
            $"IDEmisorFactura={issuerNif.Trim()}" +
            $"&NumSerieFactura={invoiceNumber.Trim()}" +
            $"&FechaExpedicionFactura={FormatDate(issueDate)}" +
            $"&TipoFactura={invoiceTypeCode}" +
            $"&CuotaTotal={FormatAmount(totalTax)}" +
            $"&ImporteTotal={FormatAmount(totalAmount)}" +
            $"&Huella={previousHash?.Trim() ?? string.Empty}" +
            $"&FechaHoraHusoGenRegistro={FormatTimestamp(generatedAt)}";

        return (input, Sha256Upper(input));
    }

    /// <summary>Hash input and digest for a RegistroAnulacion.</summary>
    public static (string Input, string Hash) ComputeAnulacion(
        string issuerNif,
        string invoiceNumber,
        DateOnly issueDate,
        string? previousHash,
        DateTimeOffset generatedAt)
    {
        var input =
            $"IDEmisorFacturaAnulada={issuerNif.Trim()}" +
            $"&NumSerieFacturaAnulada={invoiceNumber.Trim()}" +
            $"&FechaExpedicionFacturaAnulada={FormatDate(issueDate)}" +
            $"&Huella={previousHash?.Trim() ?? string.Empty}" +
            $"&FechaHoraHusoGenRegistro={FormatTimestamp(generatedAt)}";

        return (input, Sha256Upper(input));
    }

    private static string Sha256Upper(string input)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
}
