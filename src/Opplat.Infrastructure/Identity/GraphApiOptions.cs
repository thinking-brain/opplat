namespace Opplat.Infrastructure.Identity;

/// <summary>
/// Configuration for the Microsoft Graph API client (client-credentials flow).
/// Bind from <c>GraphApi</c> configuration section.
/// </summary>
public sealed class GraphApiOptions
{
    public const string SectionName = "GraphApi";
    private const int DefaultRetryCount = 3;
    private const int DefaultBaseDelaySeconds = 2;
    private const int DefaultMaxDelaySeconds = 30;

    /// <summary>Azure Entra ID tenant ID (directory ID).</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>App registration client ID.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>App registration client secret. Mutually exclusive with <see cref="CertificateThumbprint"/>.</summary>
    public string? ClientSecret { get; set; }

    /// <summary>Certificate thumbprint for certificate-based auth. Mutually exclusive with <see cref="ClientSecret"/>.</summary>
    public string? CertificateThumbprint { get; set; }

    /// <summary>
    /// The <c>.onmicrosoft.com</c> domain for UPN generation.
    /// Example: <c>mycompany.onmicrosoft.com</c>.
    /// </summary>
    public string TenantDomain { get; set; } = string.Empty;

    /// <summary>When true, the Graph client is registered and operational. When false, a no-op stub is used.</summary>
    public bool Enabled { get; set; }

    /// <summary>Number of retries for transient Graph failures (HTTP 429/503) after the initial attempt.</summary>
    public int MaxRetryAttempts { get; set; } = DefaultRetryCount;

    /// <summary>Base exponential-backoff delay, in seconds, when Graph does not return a retry hint.</summary>
    public int RetryBaseDelaySeconds { get; set; } = DefaultBaseDelaySeconds;

    /// <summary>Maximum retry delay, in seconds, when Graph does not return a retry hint.</summary>
    public int RetryMaxDelaySeconds { get; set; } = DefaultMaxDelaySeconds;
}
