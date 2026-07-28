using System.Security.Cryptography;
using System.Text;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Computes the SHA-256 "huella" (hash/fingerprint) required by the Verifactu
/// specification (RD 1007/2023, Annex II).
/// </summary>
/// <remarks>
/// <b>TODO (compliance verification required):</b> The exact field order, separator
/// character, date formats, and encoding rules MUST be verified against the official
/// AEAT PDF
/// "Algoritmo de cálculo y codificación de la huella o hash del registro de facturación"
/// from <c>sede.agenciatributaria.gob.es</c> before enabling live AEAT submission.
/// The implementation below reflects the general Verifactu public documentation but
/// byte-level details may differ from the final AEAT spec.
/// </remarks>
public static class VerifactuHashCalculator
{
    private const char Separator = '|';

    /// <summary>
    /// Builds the canonical concatenation string and returns its SHA-256 hex digest
    /// (lowercase, no dashes).
    /// </summary>
    /// <param name="issuerNif">NIF/tax-ID of the issuing tenant.</param>
    /// <param name="invoice">The invoice being hashed.</param>
    /// <param name="totalTax">Sum of all tax amounts across all tax breakdowns.</param>
    /// <param name="generatedAtUtc">Timestamp when the fiscal record is generated.</param>
    /// <param name="previousHash">Hash of the immediately preceding record in the same series/year chain; empty string for the first record.</param>
    /// <returns>A tuple of (canonicalInput, sha256HexHash).</returns>
    public static (string Input, string Hash) Compute(
        string issuerNif,
        Invoice invoice,
        decimal totalTax,
        DateTime generatedAtUtc,
        string previousHash)
    {
        // TODO: verify exact field order, date format ("dd-MM-yyyy" vs "yyyy-MM-ddTHH:mm:ss"),
        // invoice type code mapping, and whether total amounts use "." or "," decimals
        // against the official AEAT "huella" spec PDF.
        var invoiceTypeCode = invoice.InvoiceType switch
        {
            InvoiceType.Simplified => "F2",
            InvoiceType.Full => "F1",
            InvoiceType.Rectifying => "R1",
            InvoiceType.Substitutive => "F3",
            _ => "F1"
        };

        var input = string.Join(Separator,
            issuerNif,
            invoice.FullNumber,
            invoice.IssueDate.ToString("dd-MM-yyyy"),
            invoiceTypeCode,
            totalTax.ToString("0.00"),
            invoice.TotalAmount.ToString("0.00"),
            previousHash,
            generatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss"));

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)))
            .ToLowerInvariant();

        return (input, hash);
    }
}
