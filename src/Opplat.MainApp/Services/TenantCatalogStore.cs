using System.Text.Json;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Services;

public sealed class TenantCatalogStore : IMultiTenantStore<AppTenantInfo>
{
    private readonly string _catalogPath;
    private readonly List<AppTenantInfo> _seedTenants;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public TenantCatalogStore(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _seedTenants = configuration
            .GetSection("Finbuckle:MultiTenant:Stores:ConfigurationStore:Tenants")
            .Get<List<AppTenantInfo>>() ?? new List<AppTenantInfo>();

        var relativePath = configuration["TenantStore:CatalogPath"];
        _catalogPath = Path.IsPathRooted(relativePath ?? string.Empty)
            ? relativePath!
            : Path.Combine(hostEnvironment.ContentRootPath, relativePath ?? Path.Combine("Data", "tenant-catalog.json"));

        EnsureCatalogExists();
    }

    public async Task<bool> TryAddAsync(AppTenantInfo tenantInfo)
    {
        ArgumentNullException.ThrowIfNull(tenantInfo);

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            if (catalog.Tenants.Any(existing =>
                    string.Equals(existing.Identifier, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(tenantInfo.Id) &&
                     string.Equals(existing.Id, tenantInfo.Id, StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            catalog.Tenants.Add(CloneTenant(tenantInfo));
            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> TryUpdateAsync(AppTenantInfo tenantInfo)
    {
        ArgumentNullException.ThrowIfNull(tenantInfo);

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            var index = catalog.Tenants.FindIndex(existing =>
                (!string.IsNullOrWhiteSpace(tenantInfo.Id) &&
                 string.Equals(existing.Id, tenantInfo.Id, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(existing.Identifier, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
                return false;

            catalog.Tenants[index] = CloneTenant(tenantInfo);
            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> TryRemoveAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            var removed = catalog.Tenants.RemoveAll(existing =>
                string.Equals(existing.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

            if (removed == 0)
                return false;

            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<AppTenantInfo?> TryGetByIdentifierAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        var catalog = await ReadCatalogAsync();
        return catalog.Tenants
            .Where(tenant => tenant.IsActive)
            .Select(CloneTenant)
            .FirstOrDefault(tenant => string.Equals(tenant.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<AppTenantInfo?> TryGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var catalog = await ReadCatalogAsync();
        return catalog.Tenants
            .Where(tenant => tenant.IsActive)
            .Select(CloneTenant)
            .FirstOrDefault(tenant => string.Equals(tenant.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<AppTenantInfo>> GetAllAsync()
    {
        var catalog = await ReadCatalogAsync();
        return catalog.Tenants.Select(CloneTenant).ToList();
    }

    private void EnsureCatalogExists()
    {
        var directory = Path.GetDirectoryName(_catalogPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(_catalogPath))
            return;

        var initialCatalog = new TenantCatalogDocument
        {
            Tenants = _seedTenants.Select(CloneTenant).ToList()
        };

        var json = JsonSerializer.Serialize(initialCatalog, _jsonOptions);
        File.WriteAllText(_catalogPath, json);
    }

    private async Task<TenantCatalogDocument> ReadCatalogAsync()
    {
        await using var stream = File.Open(_catalogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var catalog = await JsonSerializer.DeserializeAsync<TenantCatalogDocument>(stream, _jsonOptions);

        if (catalog?.Tenants is { Count: > 0 })
            return catalog;

        return new TenantCatalogDocument
        {
            Tenants = _seedTenants.Select(CloneTenant).ToList()
        };
    }

    private async Task WriteCatalogAsync(TenantCatalogDocument catalog)
    {
        await using var stream = File.Open(_catalogPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, catalog, _jsonOptions);
    }

    private static AppTenantInfo CloneTenant(AppTenantInfo tenant) => new()
    {
        Id = tenant.Id,
        Identifier = tenant.Identifier,
        Name = tenant.Name,
        ConnectionString = tenant.ConnectionString,
        DatabaseName = tenant.DatabaseName,
        DatabaseSchema = tenant.DatabaseSchema,
        JwtSigningKey = tenant.JwtSigningKey,
        IsActive = tenant.IsActive
    };

    private sealed class TenantCatalogDocument
    {
        public List<AppTenantInfo> Tenants { get; set; } = new();
    }
}
