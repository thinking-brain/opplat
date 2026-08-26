using System.Security.Cryptography.X509Certificates;

namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class XadesSigningOptions
{
    public const string SectionName = "Invoicing:Xades";

    public string? PfxPath { get; set; }

    public string? PfxPassword { get; set; }

    public string? CertificateThumbprint { get; set; }

    public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;

    public StoreName StoreName { get; set; } = StoreName.My;
}