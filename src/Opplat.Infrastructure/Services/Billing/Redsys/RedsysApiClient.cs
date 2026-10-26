using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

public sealed record RedsysRestResult(
    bool TransportSucceeded,
    string? RawResponse,
    string? ErrorCode,
    IReadOnlyDictionary<string, string>? MerchantParameters);

/// <summary>
/// Server-to-server REST client for Redsys's <c>trataPeticionREST</c> endpoint, used only for
/// synchronous MIT (recurring) charges. The CIT card-setup flow uses a browser form POST to the
/// hosted payment page instead (see RedsysCardSetupService), not this client.
/// </summary>
public sealed class RedsysApiClient(HttpClient httpClient, IOptions<RedsysOptions> options)
{
    // Explicit: Redsys's field names are case-sensitive (Ds_SignatureVersion, ...) and must not
    // be run through System.Text.Json's default camelCase naming policy.
    private static readonly JsonSerializerOptions RequestSerializerOptions = new() { PropertyNamingPolicy = null };

    private readonly RedsysOptions _options = options.Value;

    public async Task<RedsysRestResult> PostRestTransactionAsync(
        string merchantParametersBase64,
        string signature,
        CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            Ds_SignatureVersion = "HMAC_SHA256_V1",
            Ds_MerchantParameters = merchantParametersBase64,
            Ds_Signature = signature
        };

        using var response = await httpClient.PostAsJsonAsync(
            _options.RestApiUrl, requestBody, RequestSerializerOptions, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new RedsysRestResult(false, raw, null, null);

        using var document = JsonDocument.Parse(raw);
        if (document.RootElement.TryGetProperty("errorCode", out var errorCodeElement))
            return new RedsysRestResult(false, raw, errorCodeElement.GetString(), null);

        if (!document.RootElement.TryGetProperty("Ds_MerchantParameters", out var parametersElement))
            return new RedsysRestResult(false, raw, "MISSING_MERCHANT_PARAMETERS", null);

        var responseJson = Encoding.UTF8.GetString(Convert.FromBase64String(parametersElement.GetString() ?? string.Empty));
        var responseParameters = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson)
            ?? new Dictionary<string, string>();

        return new RedsysRestResult(true, raw, null, responseParameters);
    }
}
