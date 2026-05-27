using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Identity;

namespace Opplat.UnitTest.Identity;

public class GraphUserServiceRetryTests
{
    [Fact]
    public async Task CreateUserAsync_RetriesTransient429And503Responses_AndEventuallySucceeds()
    {
        var handler = new SequenceHttpMessageHandler(
            CreateTransientGraphErrorResponse(HttpStatusCode.TooManyRequests, retryAfterSeconds: 0),
            CreateTransientGraphErrorResponse(HttpStatusCode.ServiceUnavailable, retryAfterMilliseconds: 0),
            CreateJsonResponse(HttpStatusCode.Created, """
                {
                  "id": "graph-user-oid"
                }
                """));

        var sut = CreateSut(handler, new GraphApiOptions
        {
            TenantDomain = "contoso.onmicrosoft.com",
            MaxRetryAttempts = 2
        });

        var result = await sut.CreateUserAsync(new CreateUserRequest
        {
            Email = "user@example.com",
            UserName = "Test User",
            FirstName = "Test",
            LastName = "User",
            Password = "TempP@ss1!"
        });

        Assert.True(result.Succeeded);
        Assert.Equal("graph-user-oid", result.ObjectId);
        Assert.Equal(3, handler.Requests.Count);

        foreach (var request in handler.Requests)
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/users", request.Uri, StringComparison.OrdinalIgnoreCase);
            Assert.Equal("Bearer", request.AuthorizationScheme);
            Assert.Equal("test-token", request.AuthorizationParameter);
        }

        using var body = JsonDocument.Parse(handler.Requests[0].Body!);
        Assert.Equal("Test User", body.RootElement.GetProperty("displayName").GetString());
        Assert.Equal("user@example.com", body.RootElement.GetProperty("mail").GetString());
        Assert.True(body.RootElement.GetProperty("accountEnabled").GetBoolean());
        Assert.True(body.RootElement
            .GetProperty("passwordProfile")
            .GetProperty("forceChangePasswordNextSignIn")
            .GetBoolean());
    }

    [Fact]
    public async Task DisableUserAsync_StopsAfterConfiguredRetries_AndReturnsFailure()
    {
        var handler = new SequenceHttpMessageHandler(
            CreateTransientGraphErrorResponse(HttpStatusCode.ServiceUnavailable, retryAfterMilliseconds: 0),
            CreateTransientGraphErrorResponse(HttpStatusCode.ServiceUnavailable, retryAfterMilliseconds: 0),
            CreateTransientGraphErrorResponse(HttpStatusCode.ServiceUnavailable, retryAfterMilliseconds: 0));

        var sut = CreateSut(handler, new GraphApiOptions
        {
            TenantDomain = "contoso.onmicrosoft.com",
            MaxRetryAttempts = 2
        });

        var result = await sut.DisableUserAsync("user-123");

        Assert.False(result.Succeeded);
        Assert.Equal(503, result.StatusCode);
        Assert.Contains("DisableUser failed", result.Error);
        Assert.Equal(3, handler.Requests.Count);

        foreach (var request in handler.Requests)
        {
            Assert.Equal(HttpMethod.Patch, request.Method);
            Assert.EndsWith("/users/user-123", request.Uri, StringComparison.OrdinalIgnoreCase);
        }

        using var body = JsonDocument.Parse(handler.Requests[0].Body!);
        Assert.False(body.RootElement.GetProperty("accountEnabled").GetBoolean());
    }

    private static GraphUserService CreateSut(SequenceHttpMessageHandler handler, GraphApiOptions options)
    {
        var httpClient = new HttpClient(handler);
        var graphClient = new GraphServiceClient(httpClient, new TestTokenCredential(), ["https://graph.microsoft.com/.default"]);

        return new GraphUserService(
            graphClient,
            Options.Create(options),
            logger: Microsoft.Extensions.Logging.Abstractions.NullLogger<GraphUserService>.Instance,
            TimeProvider.System);
    }

    private static HttpResponseMessage CreateTransientGraphErrorResponse(
        HttpStatusCode statusCode,
        int? retryAfterSeconds = null,
        int? retryAfterMilliseconds = null)
    {
        var response = CreateJsonResponse(statusCode, """
            {
              "error": {
                "code": "TransientFailure",
                "message": "Please retry later."
              }
            }
            """);

        if (retryAfterSeconds.HasValue)
            response.Headers.TryAddWithoutValidation("Retry-After", retryAfterSeconds.Value.ToString());

        if (retryAfterMilliseconds.HasValue)
            response.Headers.TryAddWithoutValidation("x-ms-retry-after-ms", retryAfterMilliseconds.Value.ToString());

        return response;
    }

    private static HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode, string json) =>
        new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    private sealed class TestTokenCredential : TokenCredential
    {
        private static readonly AccessToken AccessToken =
            new("test-token", DateTimeOffset.UtcNow.AddHours(1));

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            AccessToken;

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            ValueTask.FromResult(AccessToken);
    }

    private sealed class SequenceHttpMessageHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new(responses);

        public List<CapturedRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            Requests.Add(new CapturedRequest(
                request.Method,
                request.RequestUri?.ToString() ?? string.Empty,
                request.Headers.Authorization?.Scheme,
                request.Headers.Authorization?.Parameter,
                body));

            if (_responses.Count == 0)
                throw new InvalidOperationException("No fake Graph response was configured for this request.");

            return _responses.Dequeue();
        }
    }

    private sealed record CapturedRequest(
        HttpMethod Method,
        string Uri,
        string? AuthorizationScheme,
        string? AuthorizationParameter,
        string? Body);
}
