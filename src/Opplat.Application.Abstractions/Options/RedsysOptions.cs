namespace Opplat.Application.Abstractions.Options;

/// <summary>
/// Redsys merchant credentials and environment configuration. Bind from the
/// <c>Redsys</c> appsettings section. See docs/redsys_net_integration_guide.md for the
/// high-level integration design this options class supports.
/// </summary>
public sealed class RedsysOptions
{
    public const string SectionName = "Redsys";

    /// <summary>
    /// When <c>false</c> (default) <see cref="Opplat.Infrastructure.DependencyInjection.ServiceCollectionExtensions.AddPaymentGatewayService"/>
    /// registers the mock gateway instead of the real Redsys implementation.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>Ds_Merchant_MerchantCode (FUC) issued by the acquiring bank.</summary>
    public string MerchantCode { get; set; } = string.Empty;

    /// <summary>Ds_Merchant_Terminal, defaults to "001".</summary>
    public string Terminal { get; set; } = "001";

    /// <summary>Base64 HMAC-SHA256 secret key from the Redsys merchant back-office.</summary>
    public string SecretKeyBase64 { get; set; } = string.Empty;

    /// <summary>Selects the sandbox vs. live Redsys endpoints.</summary>
    public RedsysEnvironment Environment { get; set; } = RedsysEnvironment.Test;

    /// <summary>Ds_Merchant_MerchantURL — the async notification (webhook) callback URL.</summary>
    public string MerchantUrl { get; set; } = string.Empty;

    /// <summary>Ds_Merchant_UrlOK — browser redirect target after a successful 3DS challenge.</summary>
    public string UrlOk { get; set; } = string.Empty;

    /// <summary>Ds_Merchant_UrlKO — browser redirect target after a failed/cancelled 3DS challenge.</summary>
    public string UrlKo { get; set; } = string.Empty;

    /// <summary>
    /// Hosted-page URL the browser is redirected/form-posted to for CIT (card-setup) requests.
    /// </summary>
    public string HostedPaymentPageUrl => Environment == RedsysEnvironment.Production
        ? "https://sis.redsys.es/sis/realizarPago"
        : "https://sis-t.redsys.es:25443/sis/realizarPago";

    /// <summary>Server-to-server REST endpoint used for MIT (recurring) charges.</summary>
    public string RestApiUrl => Environment == RedsysEnvironment.Production
        ? "https://sis.redsys.es/sis/rest/trataPeticionREST"
        : "https://sis-t.redsys.es:25443/sis/rest/trataPeticionREST";
}

public enum RedsysEnvironment
{
    Test,
    Production
}
