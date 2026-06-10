using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Opplat.Api.Admin.Auth;

public sealed class MemoryCacheTicketStore : ITicketStore
{
    private readonly IMemoryCache _cache;

    public MemoryCacheTicketStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var expiration = ticket.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddHours(8);
        _cache.Set(key, ticket, expiration);
        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        _cache.TryGetValue(key, out AuthenticationTicket? ticket);
        return Task.FromResult(ticket);
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"admin-ticket-{Guid.NewGuid():N}";
        var expiration = ticket.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddHours(8);
        _cache.Set(key, ticket, expiration);
        return Task.FromResult(key);
    }
}
