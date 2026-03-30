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
