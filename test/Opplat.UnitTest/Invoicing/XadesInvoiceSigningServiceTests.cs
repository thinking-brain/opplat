using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Extensions.Options;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing;

namespace Opplat.UnitTest.Invoicing;

public sealed class XadesInvoiceSigningServiceTests
{
    [Fact]
    public async Task SignAsync_CreatesVerifiableXadesSignature()
    {
        using var rsa = RSA.Create(2048);
        using var certificate = CreateCertificate(rsa);
        var pfxPath = Path.Combine(Path.GetTempPath(), $"opplat-xades-{Guid.NewGuid():N}.pfx");
        await File.WriteAllBytesAsync(pfxPath, certificate.Export(X509ContentType.Pkcs12, "password"));

        try
        {
            var service = new XadesInvoiceSigningService(Options.Create(new XadesSigningOptions
            {
                PfxPath = pfxPath,
                PfxPassword = "password"
            }));
            var record = new InvoiceFiscalRecord
            {
                InvoiceId = Guid.NewGuid(),
                RecordHash = "record-hash",
                HashInput = "canonical-input",
                GeneratedAtUtc = new DateTime(2026, 8, 26, 12, 0, 0, DateTimeKind.Utc),
                SoftwareName = "Opplat",
                SoftwareVersion = "1.0.0",
                SubmissionMode = FiscalSubmissionMode.NonVerifactuSigned
            };

            var signedXml = await service.SignAsync(record);
            var document = new XmlDocument { PreserveWhitespace = true };
            document.LoadXml(signedXml);
            var signatureElement = (XmlElement?)document.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];

            Assert.NotNull(signatureElement);
            Assert.NotNull(record.SignatureValue);
            Assert.NotNull(document.GetElementsByTagName("SignedProperties", "http://uri.etsi.org/01903/v1.3.2#")[0]);

            var signatureValue = signatureElement!.GetElementsByTagName("SignatureValue", SignedXml.XmlDsigNamespaceUrl)[0]!.InnerText;
            Assert.NotEmpty(Convert.FromBase64String(signatureValue));
        }
        finally
        {
            File.Delete(pfxPath);
        }
    }

    [Fact]
    public async Task SignAsync_RequiresConfiguredCertificate()
    {
        var service = new XadesInvoiceSigningService(Options.Create(new XadesSigningOptions()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SignAsync(new InvoiceFiscalRecord
        {
            InvoiceId = Guid.NewGuid(),
            RecordHash = "record-hash",
            HashInput = "canonical-input",
            GeneratedAtUtc = DateTime.UtcNow,
            SoftwareName = "Opplat",
            SoftwareVersion = "1.0.0"
        }));
    }

    private static X509Certificate2 CreateCertificate(RSA rsa)
        => new CertificateRequest(
            "CN=Opplat XAdES Test",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1)
            .CreateSelfSigned(
                DateTimeOffset.UtcNow.AddMinutes(-1),
                DateTimeOffset.UtcNow.AddMinutes(10));
}