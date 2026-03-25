using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Opplat.MainApp.Auth;

public sealed class MemoryCacheTicketStore : ITicketStore
{
    private const string KeyPrefix = "opplat-admin-session-";
    private readonly IMemoryCache _cache;

    public MemoryCacheTicketStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket) =>
        StoreAsync(ticket, CancellationToken.None);

    public Task<string> StoreAsync(AuthenticationTicket ticket, CancellationToken cancellationToken)
    {
        var key = $"{KeyPrefix}{Guid.NewGuid():N}";
        return StoreAsyncCore(key, ticket);
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket, HttpContext context, CancellationToken cancellationToken) =>
        StoreAsync(ticket, cancellationToken);

    public Task RenewAsync(string key, AuthenticationTicket ticket) =>
        RenewAsync(key, ticket, CancellationToken.None);

    public async Task RenewAsync(string key, AuthenticationTicket ticket, CancellationToken cancellationToken) =>
        await StoreAsyncCore(key, ticket);

    public Task RenewAsync(string key, AuthenticationTicket ticket, HttpContext context, CancellationToken cancellationToken) =>
        RenewAsync(key, ticket, cancellationToken);

    public Task<AuthenticationTicket?> RetrieveAsync(string key) =>
        RetrieveAsync(key, CancellationToken.None);

    public Task<AuthenticationTicket?> RetrieveAsync(string key, CancellationToken cancellationToken)
    {
        _cache.TryGetValue(key, out AuthenticationTicket? ticket);
        return Task.FromResult(ticket);
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key, HttpContext context, CancellationToken cancellationToken) =>
        RetrieveAsync(key, cancellationToken);

    public Task RemoveAsync(string key) =>
        RemoveAsync(key, CancellationToken.None);

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, HttpContext context, CancellationToken cancellationToken) =>
        RemoveAsync(key, cancellationToken);

    private Task<string> StoreAsyncCore(string key, AuthenticationTicket ticket)
    {
        var expiresAt = ticket.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddHours(8);
        _cache.Set(key, ticket, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiresAt
        });

        return Task.FromResult(key);
    }
}
