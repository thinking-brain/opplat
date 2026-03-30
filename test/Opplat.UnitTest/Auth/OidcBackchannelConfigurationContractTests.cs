using System.Text.RegularExpressions;

namespace Opplat.UnitTest.Auth;

public class OidcBackchannelConfigurationContractTests
{
    [Fact]
    public void BackendOidcConfiguration_SupportsSeparateDiscoveryMetadataAddress()
    {
        var authOptions = TestRepository.ReadAllText("src", "Opplat.MainApp", "Auth", "AuthOptions.cs");
        var program = TestRepository.ReadAllText("src", "Opplat.MainApp", "Program.cs");

        Assert.Contains("public string? MetadataAddress { get; set; }", authOptions);
        Assert.Contains("options.MetadataAddress = authOptions.MetadataAddress;", program);
        Assert.Contains("ValidIssuer = authOptions.Authority,", program);
    }

    [Fact]
    public void DockerCompose_AlignsPublicIssuerWithInternalKeycloakBackchannelDiscovery()
    {
        const string publicAuthority = "Auth__Authority=${AUTH__AUTHORITY:-http://localhost:${KEYCLOAK_PORT:-8180}/realms/${KEYCLOAK_REALM:-opplat}}";
        const string metadataAddress = "Auth__MetadataAddress=${AUTH__METADATA_ADDRESS:-http://keycloak:8180/realms/${KEYCLOAK_REALM:-opplat}/.well-known/openid-configuration}";
        const string keycloakCommand = "start-dev --import-realm --hostname=http://localhost:${KEYCLOAK_PORT:-8180} --hostname-backchannel-dynamic=true";

        var compose = TestRepository.ReadAllText("docker-compose.yml");

        Assert.Contains(keycloakCommand, compose);
        Assert.Equal(4, Regex.Count(compose, Regex.Escape(publicAuthority)));
        Assert.Equal(4, Regex.Count(compose, Regex.Escape(metadataAddress)));
    }
}
