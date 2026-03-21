using System.IO;
using System.Text.RegularExpressions;

namespace Opplat.MainApp.Test.Auth;

public class OidcBackchannelConfigurationContractTests
{
    [Fact]
    public void BackendOidcConfiguration_SupportsSeparateDiscoveryMetadataAddress()
    {
        var authOptions = File.ReadAllText(ResolveRepoFile("src", "Opplat.MainApp", "Auth", "AuthOptions.cs"));
        var program = File.ReadAllText(ResolveRepoFile("src", "Opplat.MainApp", "Program.cs"));

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

        var compose = File.ReadAllText(ResolveRepoFile("docker-compose.yml"));

        Assert.Contains(keycloakCommand, compose);
        Assert.Equal(3, Regex.Matches(compose, Regex.Escape(publicAuthority)).Count);
        Assert.Equal(3, Regex.Matches(compose, Regex.Escape(metadataAddress)).Count);
    }

    private static string ResolveRepoFile(params string[] segments)
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "opplat.sln")))
            {
                return Path.Combine(new[] { current }.Concat(segments).ToArray());
            }

            current = Directory.GetParent(current)?.FullName!;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
