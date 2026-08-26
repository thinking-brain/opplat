using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class XadesInvoiceSigningService(IOptions<XadesSigningOptions> options)
    : IInvoiceSigningService
{
    private const string XadesNamespace = "http://uri.etsi.org/01903/v1.3.2#";
    private const string XmlDsigNamespace = SignedXml.XmlDsigNamespaceUrl;

    public Task<string> SignAsync(
        InvoiceFiscalRecord record,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        cancellationToken.ThrowIfCancellationRequested();

        using var certificate = LoadCertificate(options.Value);
        var document = CreateFiscalRecordDocument(record);
        var signatureId = $"Signature-{record.Id:N}";
        var signedPropertiesId = $"SignedProperties-{record.Id:N}";
        using var privateKey = certificate.GetRSAPrivateKey()
            ?? throw new InvalidOperationException("The XAdES certificate must contain an RSA private key.");

        var signedXml = new XadesSignedXml(document)
        {
            SigningKey = privateKey
        };
        var signature = signedXml.Signature
            ?? throw new InvalidOperationException("The XML signature could not be initialized.");
        var signedInfo = signature.SignedInfo
            ?? throw new InvalidOperationException("The XML signature information could not be initialized.");
        signature.Id = signatureId;
        signedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
        signedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

        var documentReference = new Reference
        {
            Uri = $"#Record-{record.Id:N}",
            DigestMethod = SignedXml.XmlDsigSHA256Url
        };
        documentReference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        documentReference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(documentReference);

        var qualifyingProperties = CreateQualifyingProperties(
            document,
            certificate,
            signatureId,
            signedPropertiesId);
        var signedPropertiesReference = new Reference
        {
            Uri = $"#{signedPropertiesId}",
            Type = XadesNamespace + "SignedProperties",
            DigestMethod = SignedXml.XmlDsigSHA256Url
        };
        signedPropertiesReference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(signedPropertiesReference);
        var objectContainer = document.CreateElement("xades", "ObjectContainer", XadesNamespace);
        objectContainer.AppendChild(qualifyingProperties);
        document.DocumentElement!.AppendChild(objectContainer);
        signedXml.AddObject(new DataObject { Data = objectContainer.ChildNodes });
        document.DocumentElement.RemoveChild(objectContainer);
        signedXml.KeyInfo = CreateKeyInfo(certificate);
        signedXml.ComputeSignature();

        document.DocumentElement!.AppendChild(document.ImportNode(signedXml.GetXml(), true));
        record.SignatureValue = signature.SignatureValue is null
            ? throw new InvalidOperationException("The XML signature did not produce a signature value.")
            : Convert.ToBase64String(signature.SignatureValue);

        return Task.FromResult(document.OuterXml);
    }

    private static XmlDocument CreateFiscalRecordDocument(InvoiceFiscalRecord record)
    {
        var document = new XmlDocument { PreserveWhitespace = true };
        var root = document.CreateElement("sif", "FiscalRecord", "urn:opplat:fiscal-record");
        root.SetAttribute("Id", $"Record-{record.Id:N}");
        document.AppendChild(root);

        AddElement(document, root, "InvoiceId", record.InvoiceId.ToString("D"));
        AddElement(document, root, "PreviousRecordHash", record.PreviousRecordHash);
        AddElement(document, root, "RecordHash", record.RecordHash);
        AddElement(document, root, "HashInput", record.HashInput);
        AddElement(document, root, "GeneratedAtUtc", record.GeneratedAtUtc.ToUniversalTime().ToString("O"));
        AddElement(document, root, "SoftwareName", record.SoftwareName);
        AddElement(document, root, "SoftwareVersion", record.SoftwareVersion);
        AddElement(document, root, "SoftwareLicenseId", record.SoftwareLicenseId);
        return document;
    }

    private static XmlElement CreateQualifyingProperties(
        XmlDocument document,
        X509Certificate2 certificate,
        string signatureId,
        string signedPropertiesId)
    {
        var qualifyingProperties = document.CreateElement("xades", "QualifyingProperties", XadesNamespace);
        qualifyingProperties.SetAttribute("Target", $"#{signatureId}");

        var signedProperties = document.CreateElement("xades", "SignedProperties", XadesNamespace);
        signedProperties.SetAttribute("Id", signedPropertiesId);
        qualifyingProperties.AppendChild(signedProperties);
        var signedSignatureProperties = document.CreateElement("xades", "SignedSignatureProperties", XadesNamespace);
        signedProperties.AppendChild(signedSignatureProperties);

        AddElement(document, signedSignatureProperties, "SigningTime", DateTime.UtcNow.ToString("O"), XadesNamespace);
        var signingCertificate = document.CreateElement("xades", "SigningCertificate", XadesNamespace);
        signedSignatureProperties.AppendChild(signingCertificate);
        var cert = document.CreateElement("xades", "Cert", XadesNamespace);
        signingCertificate.AppendChild(cert);
        var certDigest = document.CreateElement("xades", "CertDigest", XadesNamespace);
        cert.AppendChild(certDigest);
        AddElement(document, certDigest, "DigestMethod", null, XmlDsigNamespace)
            .SetAttribute("Algorithm", SignedXml.XmlDsigSHA256Url);
        AddElement(document, certDigest, "DigestValue", Convert.ToBase64String(SHA256.HashData(certificate.RawData)), XmlDsigNamespace);
        var issuerSerial = document.CreateElement("xades", "IssuerSerial", XadesNamespace);
        cert.AppendChild(issuerSerial);
        AddElement(document, issuerSerial, "X509IssuerName", certificate.Issuer, XmlDsigNamespace);
        AddElement(document, issuerSerial, "X509SerialNumber", certificate.SerialNumber, XmlDsigNamespace);
        return qualifyingProperties;
    }

    private static KeyInfo CreateKeyInfo(X509Certificate2 certificate)
    {
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(certificate));
        return keyInfo;
    }

    private static X509Certificate2 LoadCertificate(XadesSigningOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.PfxPath))
        {
            return X509CertificateLoader.LoadPkcs12FromFile(
                options.PfxPath,
                options.PfxPassword,
                X509KeyStorageFlags.EphemeralKeySet);
        }

        if (string.IsNullOrWhiteSpace(options.CertificateThumbprint))
            throw new InvalidOperationException(
                $"Configure {XadesSigningOptions.SectionName}:PfxPath or CertificateThumbprint to enable XAdES signing.");

        using var store = new X509Store(options.StoreName, options.StoreLocation);
        store.Open(OpenFlags.ReadOnly);
        var thumbprint = options.CertificateThumbprint.Replace(" ", string.Empty, StringComparison.Ordinal);
        var certificate = store.Certificates.Find(
            X509FindType.FindByThumbprint,
            thumbprint,
            validOnly: false)
            .OfType<X509Certificate2>()
            .FirstOrDefault();
        return certificate is null
            ? throw new InvalidOperationException($"Certificate '{thumbprint}' was not found in the configured certificate store.")
            : new X509Certificate2(certificate);
    }

    private static XmlElement AddElement(
        XmlDocument document,
        XmlNode parent,
        string name,
        string? value,
        string? namespaceUri = null)
    {
        var element = document.CreateElement(name, namespaceUri ?? parent.NamespaceURI);
        if (value is not null)
            element.InnerText = value;
        parent.AppendChild(element);
        return element;
    }

    public sealed class XadesSignedXml(XmlDocument document) : SignedXml(document)
    {
        public override XmlElement? GetIdElement(XmlDocument? document, string idValue)
        {
            var element = document is null ? null : base.GetIdElement(document, idValue);
            if (element is not null)
                return element;

            foreach (DataObject dataObject in Signature.ObjectList)
            {
                foreach (XmlNode node in dataObject.Data)
                {
                    if (node is XmlElement candidate)
                    {
                        if (candidate.GetAttribute("Id") == idValue)
                            return candidate;

                        var descendant = candidate.SelectSingleNode($".//*[@Id='{idValue}']") as XmlElement;
                        if (descendant is not null)
                            return descendant;
                    }
                }
            }

            return null;
        }
    }
}