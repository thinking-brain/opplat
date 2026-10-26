namespace Opplat.Application.Abstractions.Services;

/// <summary>
/// Additive interface for the Redsys CIT (Customer-Initiated Transaction) card-setup flow.
/// Deliberately separate from <see cref="IPaymentGatewayService"/> because real card
/// tokenization requires a browser redirect + 3DS challenge, not a synchronous server call.
/// </summary>
public interface IRedsysCardSetupService
{
    /// <summary>
    /// Generates a signed Ds_Order/Ds_MerchantParameters/Ds_Signature payload for the tenant's
    /// first billing period and returns the hosted-page URL + form fields the caller should
    /// auto-submit (as an HTML form POST) to redirect the browser to Redsys's 3DS challenge.
    /// </summary>
    Task<RedsysCardSetupRedirect> InitiateCardSetupAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies and processes an asynchronous Redsys notification (webhook). The caller is
    /// responsible for parsing the incoming form body into <paramref name="notificationFields"/>
    /// before calling this method.
    /// </summary>
    Task<RedsysNotificationResult> HandleNotificationAsync(
        IReadOnlyDictionary<string, string> notificationFields,
        CancellationToken cancellationToken = default);
}

public sealed record RedsysCardSetupRedirect(string ActionUrl, IReadOnlyDictionary<string, string> FormFields);

public sealed record RedsysNotificationResult(bool Recognized, bool Authorized, string? Order, string? Error);
