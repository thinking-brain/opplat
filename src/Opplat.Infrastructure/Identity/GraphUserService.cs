using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// Graph API user-lifecycle service using client-credentials flow.
/// UPNs follow the collision-safe format <c>{uuid}@{TenantDomain}</c>.
/// All operations are async with explicit retry handling for transient 429/503 failures.
/// </summary>
public sealed class GraphUserService(
    GraphServiceClient client,
    IOptions<GraphApiOptions> options,
    ILogger<GraphUserService> logger,
    TimeProvider timeProvider) : IUserManagementService
{
    private readonly GraphServiceClient _client = client;
    private readonly GraphApiOptions _options = options.Value;
    private readonly ILogger<GraphUserService> _logger = logger;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var upn = $"{Guid.NewGuid()}@{_options.TenantDomain}";

        var user = new User
        {
            AccountEnabled = true,
            DisplayName = request.UserName,
            GivenName = request.FirstName,
            Surname = request.LastName,
            MailNickname = upn.Split('@')[0],
            UserPrincipalName = upn,
            Mail = request.Email,
            OtherMails = [request.Email],
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = request.Password
            }
        };

        try
        {
            var created = await ExecuteWithRetryAsync(
                "CreateUser",
                upn,
                retryCt => _client.Users.PostAsync(user, cancellationToken: retryCt),
                ct);

            _logger.LogInformation("Created Entra user {Oid} with UPN {Upn}", created?.Id, upn);
            return UserOperationResult.Success(created?.Id);
        }
        catch (ODataError ex)
        {
            return HandleODataError(ex, "CreateUser", upn);
        }
    }

    public async Task<UserOperationResult> EnableUserAsync(string objectId, CancellationToken ct = default)
    {
        return await PatchAccountEnabled(objectId, true, ct);
    }

    public async Task<UserOperationResult> DisableUserAsync(string objectId, CancellationToken ct = default)
    {
        return await PatchAccountEnabled(objectId, false, ct);
    }

    public async Task<UserOperationResult> DeleteUserAsync(string objectId, CancellationToken ct = default)
    {
        try
        {
            await ExecuteWithRetryAsync(
                "DeleteUser",
                objectId,
                retryCt => _client.Users[objectId].DeleteAsync(cancellationToken: retryCt),
                ct);

            _logger.LogInformation("Deleted Entra user {Oid}", objectId);
            return UserOperationResult.Success(objectId);
        }
        catch (ODataError ex)
        {
            return HandleODataError(ex, "DeleteUser", objectId);
        }
    }

    public async Task<UserOperationResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default)
    {
        try
        {
            await ExecuteWithRetryAsync(
                "ResetPassword",
                objectId,
                retryCt => _client.Users[objectId].PatchAsync(new User
                {
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = temporaryPassword
                    }
                }, cancellationToken: retryCt),
                ct);

            _logger.LogInformation("Reset password for Entra user {Oid}", objectId);
            return UserOperationResult.Success(objectId);
        }
        catch (ODataError ex)
        {
            return HandleODataError(ex, "ResetPassword", objectId);
        }
    }

    private async Task<UserOperationResult> PatchAccountEnabled(string objectId, bool enabled, CancellationToken ct)
    {
        try
        {
            await ExecuteWithRetryAsync(
                enabled ? "EnableUser" : "DisableUser",
                objectId,
                retryCt => _client.Users[objectId].PatchAsync(new User
                {
                    AccountEnabled = enabled
                }, cancellationToken: retryCt),
                ct);

            _logger.LogInformation("{Action} Entra user {Oid}", enabled ? "Enabled" : "Disabled", objectId);
            return UserOperationResult.Success(objectId);
        }
        catch (ODataError ex)
        {
            return HandleODataError(ex, enabled ? "EnableUser" : "DisableUser", objectId);
        }
    }

    private UserOperationResult HandleODataError(ODataError ex, string operation, string target)
    {
        var statusCode = ex.ResponseStatusCode;
        var message = ex.Error?.Message ?? ex.Message;

        _logger.LogError(ex, "Graph API {Operation} failed for {Target}: {StatusCode} {Message}",
            operation, target, statusCode, message);

        return UserOperationResult.Failure(
            $"{operation} failed: {message}",
            statusCode);
    }

    private async Task<T> ExecuteWithRetryAsync<T>(
        string operation,
        string target,
        Func<CancellationToken, Task<T>> action,
        CancellationToken ct)
    {
        var maxRetryAttempts = Math.Max(0, _options.MaxRetryAttempts);

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                return await action(ct);
            }
            catch (ODataError ex) when (IsTransientGraphFailure(ex) && attempt < maxRetryAttempts)
            {
                var delay = GetRetryDelay(ex, attempt);

                _logger.LogWarning(
                    ex,
                    "Transient Graph API {Operation} failure for {Target}. Retrying attempt {Attempt} of {MaxRetries} after {DelayMs}ms. Status {StatusCode}.",
                    operation,
                    target,
                    attempt + 1,
                    maxRetryAttempts,
                    delay.TotalMilliseconds,
                    ex.ResponseStatusCode);

                await Task.Delay(delay, _timeProvider, ct);
            }
            catch (ODataError ex) when (attempt > 0)
            {
                _logger.LogError(
                    ex,
                    "Graph API {Operation} exhausted retries for {Target} after {Attempts} attempts.",
                    operation,
                    target,
                    attempt + 1);

                throw;
            }
        }
    }

    private async Task ExecuteWithRetryAsync(
        string operation,
        string target,
        Func<CancellationToken, Task> action,
        CancellationToken ct)
    {
        await ExecuteWithRetryAsync<object?>(
            operation,
            target,
            async retryCt =>
            {
                await action(retryCt);
                return null;
            },
            ct);
    }

    private static bool IsTransientGraphFailure(ODataError ex) =>
        ex.ResponseStatusCode is 429 or 503;

    private TimeSpan GetRetryDelay(ODataError ex, int attempt)
    {
        if (TryGetRetryDelayFromHeaders(ex.ResponseHeaders, out var retryDelay))
            return retryDelay;

        var baseDelaySeconds = Math.Max(1, _options.RetryBaseDelaySeconds);
        var maxDelaySeconds = Math.Max(baseDelaySeconds, _options.RetryMaxDelaySeconds);
        var delaySeconds = Math.Min(maxDelaySeconds, baseDelaySeconds * Math.Pow(2, attempt));

        return TimeSpan.FromSeconds(delaySeconds);
    }

    private static bool TryGetRetryDelayFromHeaders(
        IDictionary<string, IEnumerable<string>>? headers,
        out TimeSpan delay)
    {
        delay = default;
        if (headers is null || headers.Count == 0)
            return false;

        if (TryGetHeaderValues(headers, "Retry-After", out var retryAfterValues))
        {
            foreach (var value in retryAfterValues)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var retryAfterSeconds)
                    && retryAfterSeconds >= 0)
                {
                    delay = TimeSpan.FromSeconds(retryAfterSeconds);
                    return true;
                }

                if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var retryAfterDate))
                {
                    var remaining = retryAfterDate - DateTimeOffset.UtcNow;
                    if (remaining > TimeSpan.Zero)
                    {
                        delay = remaining;
                        return true;
                    }
                }
            }
        }

        if (TryGetHeaderValues(headers, "x-ms-retry-after-ms", out var retryAfterMsValues))
        {
            foreach (var value in retryAfterMsValues)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var retryAfterMs)
                    && retryAfterMs >= 0)
                {
                    delay = TimeSpan.FromMilliseconds(retryAfterMs);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryGetHeaderValues(
        IDictionary<string, IEnumerable<string>> headers,
        string name,
        out IEnumerable<string> values)
    {
        foreach (var header in headers)
        {
            if (string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                values = header.Value;
                return true;
            }
        }

        values = [];
        return false;
    }

    public Task<UserOperationResult> AssignRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
