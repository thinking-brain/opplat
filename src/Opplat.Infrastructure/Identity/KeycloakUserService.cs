using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// Keycloak Admin REST API user-lifecycle service.
/// Acquires an admin token from the master realm on each operation.
/// </summary>
public sealed class KeycloakUserService(
    HttpClient httpClient,
    IOptions<KeycloakAdminOptions> options,
    ILogger<KeycloakUserService> logger) : IKeycloakUserService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly KeycloakAdminOptions _options = options.Value;
    private readonly ILogger<KeycloakUserService> _logger = logger;

    public async Task<KeycloakUserResult> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAdminTokenAsync(ct);
            if (token is null)
                return KeycloakUserResult.Failure("Failed to obtain Keycloak admin token.");

            var userPayload = new
            {
                username = request.Username,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = request.Enabled,
                emailVerified = request.EmailVerified,
                credentials = new[]
                {
                    new { type = "password", value = request.Password, temporary = false }
                }
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(userPayload),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(httpRequest, ct);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var location = response.Headers.Location?.ToString();
                var userId = location?.Split('/').LastOrDefault();
                _logger.LogInformation("Created Keycloak user {Username} with id {UserId}", request.Username, userId);
                return KeycloakUserResult.Success(userId);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return KeycloakUserResult.Failure("A user with this username or email already exists.");

            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Keycloak CreateUser failed {Status}: {Body}", response.StatusCode, body);
            return KeycloakUserResult.Failure($"Keycloak returned {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error creating Keycloak user {Username}", request.Username);
            return KeycloakUserResult.Failure("An unexpected error occurred.");
        }
    }

    public async Task<KeycloakUserResult> DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAdminTokenAsync(ct);
            if (token is null)
                return KeycloakUserResult.Failure("Failed to obtain Keycloak admin token.");

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(httpRequest, ct);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Deleted Keycloak user {UserId}", userId);
                return KeycloakUserResult.Success(userId);
            }

            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Keycloak DeleteUser failed {Status}: {Body}", response.StatusCode, body);
            return KeycloakUserResult.Failure($"Keycloak returned {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error deleting Keycloak user {UserId}", userId);
            return KeycloakUserResult.Failure("An unexpected error occurred.");
        }
    }

    public async Task<KeycloakUserResult> AssignRealmRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAdminTokenAsync(ct);
            if (token is null)
                return KeycloakUserResult.Failure("Failed to obtain Keycloak admin token.");

            // Resolve each role name to its {id, name} representation required by the Keycloak API.
            var roleRepresentations = new List<object>();
            foreach (var roleName in roleNames)
            {
                using var roleRequest = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"{_options.BaseUrl}/admin/realms/{_options.Realm}/roles/{Uri.EscapeDataString(roleName)}");
                roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var roleResponse = await _httpClient.SendAsync(roleRequest, ct);
                if (!roleResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Keycloak role '{RoleName}' not found: {Status}", roleName, roleResponse.StatusCode);
                    return KeycloakUserResult.Failure($"Role '{roleName}' not found in Keycloak realm.");
                }

                var roleJson = await roleResponse.Content.ReadAsStringAsync(ct);
                using var roleDoc = JsonDocument.Parse(roleJson);
                var roleId = roleDoc.RootElement.GetProperty("id").GetString();
                var rolNameFromResponse = roleDoc.RootElement.GetProperty("name").GetString();
                roleRepresentations.Add(new { id = roleId, name = rolNameFromResponse });
            }

            if (roleRepresentations.Count == 0)
                return KeycloakUserResult.Success(userId);

            using var assignRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm");
            assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            assignRequest.Content = new StringContent(
                JsonSerializer.Serialize(roleRepresentations),
                Encoding.UTF8,
                "application/json");

            using var assignResponse = await _httpClient.SendAsync(assignRequest, ct);
            if (assignResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Assigned realm roles [{Roles}] to Keycloak user {UserId}",
                    string.Join(", ", roleNames), userId);
                return KeycloakUserResult.Success(userId);
            }

            var body = await assignResponse.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Keycloak role assignment failed {Status}: {Body}", assignResponse.StatusCode, body);
            return KeycloakUserResult.Failure($"Keycloak returned {(int)assignResponse.StatusCode} during role assignment.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error assigning roles to Keycloak user {UserId}", userId);
            return KeycloakUserResult.Failure("An unexpected error occurred during role assignment.");
        }
    }

    private async Task<string?> GetAdminTokenAsync(CancellationToken ct)
    {
        var tokenEndpoint = $"{_options.BaseUrl}/realms/master/protocol/openid-connect/token";
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _options.AdminUsername,
            ["password"] = _options.AdminPassword
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(formData)
        };

        using var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to get Keycloak admin token: {Status}", response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("access_token", out var tokenEl) ? tokenEl.GetString() : null;
    }
}
