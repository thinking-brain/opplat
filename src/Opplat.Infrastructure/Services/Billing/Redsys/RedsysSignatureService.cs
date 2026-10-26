using System.Security.Cryptography;
using System.Text;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

/// <summary>
/// Implements Redsys's HMAC_SHA256_V1 signature scheme: derive an order-specific key by
/// 3DES-CBC encrypting the Ds_Order with the merchant secret key (zero IV, zero padding),
/// then HMAC-SHA256 the base64 Ds_MerchantParameters payload with that derived key.
/// Byte-level padding/trimming should be re-verified against Redsys's official REST
/// interface manual before go-live (see docs/redsys_net_integration_guide.md Further
/// Considerations).
/// </summary>
public sealed class RedsysSignatureService
{
    public string ComputeSignature(string secretKeyBase64, string order, string merchantParametersBase64)
    {
        var orderKey = DeriveOrderKey(secretKeyBase64, order);
        using var hmac = new HMACSHA256(orderKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(merchantParametersBase64));
        return Convert.ToBase64String(hash);
    }

    public bool VerifySignature(
        string secretKeyBase64,
        string order,
        string merchantParametersBase64,
        string receivedSignature)
    {
        var expected = ComputeSignature(secretKeyBase64, order, merchantParametersBase64);
        var received = FromUrlSafeBase64(receivedSignature);

        byte[] expectedBytes, receivedBytes;
        try
        {
            expectedBytes = Convert.FromBase64String(expected);
            receivedBytes = Convert.FromBase64String(received);
        }
        catch (FormatException)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, receivedBytes);
    }

    private static byte[] DeriveOrderKey(string secretKeyBase64, string order)
    {
        var key = Convert.FromBase64String(secretKeyBase64);
        var orderBytes = Encoding.UTF8.GetBytes(order);

        using var tripleDes = TripleDES.Create();
        tripleDes.Mode = CipherMode.CBC;
        tripleDes.Padding = PaddingMode.Zeros;
        tripleDes.Key = key;
        tripleDes.IV = new byte[8];

        using var encryptor = tripleDes.CreateEncryptor();
        return encryptor.TransformFinalBlock(orderBytes, 0, orderBytes.Length);
    }

    /// <summary>Redsys notifications encode Ds_Signature with the URL-safe base64 alphabet.</summary>
    private static string FromUrlSafeBase64(string value) =>
        value.Replace('-', '+').Replace('_', '/');
}
