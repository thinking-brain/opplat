namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Configuration for the AEAT Verifactu SOAP submission client.
/// Bind from the <c>VerifactuSubmission</c> appsettings section.
/// </summary>
public sealed class VerifactuSubmissionOptions
{
    public const string SectionName = "VerifactuSubmission";

    /// <summary>
    /// When <c>false</c> (default) the submission client is disabled and no network
    /// calls are made to AEAT.  Enable only after validating the implementation
    /// against the AEAT pre-production portal.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// AEAT Verifactu SOAP endpoint.
    /// Pre-production: https://prewww1.aeat.es/wlpl/TIKE-CONT/ws/SistemaFacturacion/VerifactuSOAP
    /// Production:     https://www1.aeat.es/wlpl/TIKE-CONT/ws/SistemaFacturacion/VerifactuSOAP
    /// </summary>
    public string EndpointUrl { get; set; } =
        "https://prewww1.aeat.es/wlpl/TIKE-CONT/ws/SistemaFacturacion/VerifactuSOAP";
}
