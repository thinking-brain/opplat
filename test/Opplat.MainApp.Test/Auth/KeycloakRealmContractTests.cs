using System.Text.Json;

namespace Opplat.MainApp.Test.Auth;

public class KeycloakRealmContractTests
{
    [Fact]
    public void RealmImport_DefinesRequestedPlatformRoles()
    {
        using var realm = LoadRealmDocument();
        var roles = realm.RootElement
            .GetProperty("roles")
            .GetProperty("realm")
            .EnumerateArray()
            .Select(role => role.GetProperty("name").GetString())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        Assert.True(roles.SetEquals(["SuperAdmin", "TenantAdmin", "TenantUser"]));
    }

    [Fact]
    public void RealmImport_SeedsDocumentedUsersWithTenantClaimsAndRoles()
    {
        using var realm = LoadRealmDocument();
        var users = realm.RootElement
            .GetProperty("users")
            .EnumerateArray()
            .ToDictionary(
                user => user.GetProperty("username").GetString()!,
                user => user,
                StringComparer.Ordinal);

        AssertSeededUser(users, "superadmin", null, null, "SuperAdmin");
        AssertSeededUser(users, "tenant-admin@mojocafe", "mojocafe", "mojocafe", "TenantAdmin", "TenantUser");
        AssertSeededUser(users, "tenant-admin@demo", "demo", "demo", "TenantAdmin", "TenantUser");
        AssertSeededUser(users, "tenant-admin@test", "test", "test", "TenantAdmin", "TenantUser");
        AssertSeededUser(users, "user@mojocafe", "mojocafe", "mojocafe", "TenantUser");
        AssertSeededUser(users, "user@demo", "demo", "demo", "TenantUser");
        AssertSeededUser(users, "user@test", "test", "test", "TenantUser");
    }

    [Fact]
    public void RealmImport_AssignsTenantAudienceAndRoleScopesToBothSpaClients()
    {
        using var realm = LoadRealmDocument();
        var clients = realm.RootElement
            .GetProperty("clients")
            .EnumerateArray()
            .Where(client =>
            {
                var clientId = client.GetProperty("clientId").GetString();
                return string.Equals(clientId, "opplat-client", StringComparison.Ordinal) ||
                    string.Equals(clientId, "opplat-admin", StringComparison.Ordinal);
            })
            .ToList();

        Assert.Equal(2, clients.Count);

        foreach (var client in clients)
        {
            var defaultScopes = client.GetProperty("defaultClientScopes")
                .EnumerateArray()
                .Select(scope => scope.GetString())
                .Where(scope => !string.IsNullOrWhiteSpace(scope))
                .Cast<string>()
                .ToHashSet(StringComparer.Ordinal);

            Assert.True(defaultScopes.IsSupersetOf(["profile", "email", "roles", "opplat-tenancy", "opplat-api-audience"]));

            var optionalScopes = client.GetProperty("optionalClientScopes")
                .EnumerateArray()
                .Select(scope => scope.GetString())
                .Where(scope => !string.IsNullOrWhiteSpace(scope))
                .Cast<string>()
                .ToHashSet(StringComparer.Ordinal);

            Assert.Contains("offline_access", optionalScopes);
        }
    }

    [Fact]
    public void RealmImport_UsesSeparateSpaClientsForClientAndAdminApps()
    {
        using var realm = LoadRealmDocument();
        var clients = realm.RootElement
            .GetProperty("clients")
            .EnumerateArray()
            .Where(client =>
            {
                var clientId = client.GetProperty("clientId").GetString();
                return string.Equals(clientId, "opplat-client", StringComparison.Ordinal) ||
                    string.Equals(clientId, "opplat-admin", StringComparison.Ordinal);
            })
            .ToDictionary(
                client => client.GetProperty("clientId").GetString()!,
                client => client,
                StringComparer.Ordinal);

        Assert.Equal(2, clients.Count);
        AssertClientOrigins(clients["opplat-client"], "3100", "3200", "5173");
        AssertClientOrigins(clients["opplat-admin"], "3101", "3201", "5174");
    }

    [Fact]
    public void RealmImport_ExplicitlyDeclaresScopesReferencedBySpaClients()
    {
        using var realm = LoadRealmDocument();
        var declaredClientScopes = realm.RootElement
            .GetProperty("clientScopes")
            .EnumerateArray()
            .Select(scope => scope.GetProperty("name").GetString())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        var referencedClientScopes = realm.RootElement
            .GetProperty("clients")
            .EnumerateArray()
            .Where(client =>
            {
                var clientId = client.GetProperty("clientId").GetString();
                return string.Equals(clientId, "opplat-client", StringComparison.Ordinal) ||
                    string.Equals(clientId, "opplat-admin", StringComparison.Ordinal);
            })
            .SelectMany(client => client.GetProperty("defaultClientScopes").EnumerateArray()
                .Concat(client.GetProperty("optionalClientScopes").EnumerateArray()))
            .Select(scope => scope.GetString())
            .Where(scope => !string.IsNullOrWhiteSpace(scope))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        Assert.True(declaredClientScopes.IsSupersetOf(["web-origins", "profile", "email", "roles", "opplat-tenancy", "opplat-api-audience"]));

        var undeclaredReferencedScopes = referencedClientScopes
            .Except(["offline_access"], StringComparer.Ordinal)
            .Except(declaredClientScopes, StringComparer.Ordinal)
            .ToArray();

        Assert.Empty(undeclaredReferencedScopes);
    }

    [Fact]
    public void Readme_DocumentsSeededRolesAndDefaultAdminUser()
    {
        var readme = TestRepository.ReadAllText("README.md");

        Assert.Contains("SuperAdmin", readme);
        Assert.Contains("TenantAdmin", readme);
        Assert.Contains("TenantUser", readme);
        Assert.Contains("superadmin", readme);
        Assert.Contains("tenant-admin@mojocafe", readme);
        Assert.Contains("user@mojocafe", readme);
    }

    [Fact]
    public void FrontendExamples_DeclareClientOidcAndAdminApiVariables()
    {
        var clientExample = TestRepository.ReadAllText("src", "opplat-react", ".env.example");
        var adminExample = TestRepository.ReadAllText("src", "opplat-admin", ".env.example");

        Assert.Contains("VITE_AUTH_SCOPE=", clientExample);
        Assert.Contains("VITE_ADMIN_API_URL=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_SCOPE=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_AUTHORITY=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_CLIENT_ID=", adminExample);
    }

    private static void AssertSeededUser(
        IReadOnlyDictionary<string, JsonElement> users,
        string username,
        string? tenantId,
        string? tenantIdentifier,
        params string[] expectedRoles)
    {
        Assert.True(users.TryGetValue(username, out var user), $"Expected seeded Keycloak user '{username}'.");

        if (tenantId is not null || tenantIdentifier is not null)
        {
            var attributes = user.GetProperty("attributes");
            Assert.Equal(tenantId, attributes.GetProperty("tenant_id").EnumerateArray().Single().GetString());
            Assert.Equal(tenantIdentifier, attributes.GetProperty("tenant_identifier").EnumerateArray().Single().GetString());
        }
        else
        {
            Assert.False(user.TryGetProperty("attributes", out _));
        }

        var roles = user.GetProperty("realmRoles")
            .EnumerateArray()
            .Select(role => role.GetString())
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(expectedRoles.ToHashSet(StringComparer.Ordinal), roles);
    }

    private static void AssertClientOrigins(JsonElement client, params string[] expectedPorts)
    {
        var redirectUris = client.GetProperty("redirectUris")
            .EnumerateArray()
            .Select(uri => uri.GetString())
            .Where(uri => !string.IsNullOrWhiteSpace(uri))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        var webOrigins = client.GetProperty("webOrigins")
            .EnumerateArray()
            .Select(origin => origin.GetString())
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        foreach (var port in expectedPorts)
        {
            Assert.Contains($"http://localhost:{port}", webOrigins);
            Assert.Contains($"http://localhost:{port}", redirectUris);
            Assert.Contains($"http://localhost:{port}/*", redirectUris);
        }
    }

    private static JsonDocument LoadRealmDocument()
    {
        return JsonDocument.Parse(TestRepository.ReadAllText("docker", "keycloak", "opplat-realm.json"));
    }
}
