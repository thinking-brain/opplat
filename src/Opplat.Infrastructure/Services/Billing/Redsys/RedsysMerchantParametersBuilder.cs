using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

/// <summary>
/// Builds and decodes the base64-JSON Ds_MerchantParameters payload used both by the CIT
/// hosted-page redirect form and the MIT REST API body.
/// </summary>
public sealed class RedsysMerchantParametersBuilder
{
    private static readonly IReadOnlyDictionary<string, string> CurrencyNumericCodes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["EUR"] = "978",
            ["USD"] = "840",
            ["GBP"] = "826"
        };

    public string Build(IReadOnlyDictionary<string, string> fields)
    {
        var json = JsonSerializer.Serialize(fields);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public IReadOnlyDictionary<string, string> Decode(string merchantParametersBase64)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(merchantParametersBase64));
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }

    /// <summary>Redsys amounts are integer minor units (cents for EUR/USD/GBP).</summary>
    public static string FormatAmountMinorUnits(decimal amount) =>
        ((long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);

    /// <summary>Redsys requires the ISO-4217 numeric currency code, not the alphabetic one.</summary>
    public static string CurrencyNumericCode(string isoCurrencyCode) =>
        CurrencyNumericCodes.TryGetValue(isoCurrencyCode, out var code) ? code : CurrencyNumericCodes["EUR"];
}
