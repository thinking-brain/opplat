using Opplat.Infrastructure.Services.Billing.Redsys;

namespace Opplat.UnitTest.Billing.Redsys;

public sealed class RedsysSignatureServiceTests
{
    // NOTE: Redsys does not publish fixed HMAC_SHA256_V1 test vectors, so these tests validate
    // self-consistency (sign -> verify round-trips, tampering breaks verification) rather than
    // exact byte output. Re-validate against Redsys's official REST interface manual before
    // go-live (see docs/redsys_net_integration_guide.md Further Considerations).
    private const string SecretKeyBase64 = "Mk9m6qLKAxTKBQGb1RIkxsvCJcynv5oc"; // 24-byte 3DES key, base64

    [Fact]
    public void ComputeSignature_IsDeterministicForSameInputs()
    {
        var service = new RedsysSignatureService();

        var first = service.ComputeSignature(SecretKeyBase64, "123456789012", "eyJhIjoxfQ==");
        var second = service.ComputeSignature(SecretKeyBase64, "123456789012", "eyJhIjoxfQ==");

        Assert.Equal(first, second);
    }

    [Fact]
    public void VerifySignature_RoundTripsForFreshlyComputedSignature()
    {
        var service = new RedsysSignatureService();
        const string order = "123456789012";
        const string merchantParameters = "eyJhIjoxfQ==";

        var signature = service.ComputeSignature(SecretKeyBase64, order, merchantParameters);

        Assert.True(service.VerifySignature(SecretKeyBase64, order, merchantParameters, signature));
    }

    [Fact]
    public void VerifySignature_AcceptsUrlSafeBase64Variant()
    {
        var service = new RedsysSignatureService();
        const string order = "123456789012";
        const string merchantParameters = "eyJhIjoxfQ==";

        var signature = service.ComputeSignature(SecretKeyBase64, order, merchantParameters);
        var urlSafeSignature = signature.Replace('+', '-').Replace('/', '_');

        Assert.True(service.VerifySignature(SecretKeyBase64, order, merchantParameters, urlSafeSignature));
    }

    [Fact]
    public void VerifySignature_FailsWhenMerchantParametersAreTampered()
    {
        var service = new RedsysSignatureService();
        const string order = "123456789012";
        var signature = service.ComputeSignature(SecretKeyBase64, order, "eyJhIjoxfQ==");

        Assert.False(service.VerifySignature(SecretKeyBase64, order, "eyJhIjoyfQ==", signature));
    }

    [Fact]
    public void VerifySignature_FailsWhenOrderIsTampered()
    {
        var service = new RedsysSignatureService();
        const string merchantParameters = "eyJhIjoxfQ==";
        var signature = service.ComputeSignature(SecretKeyBase64, "123456789012", merchantParameters);

        Assert.False(service.VerifySignature(SecretKeyBase64, "999999999999", merchantParameters, signature));
    }
}
