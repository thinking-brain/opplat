using Microsoft.EntityFrameworkCore;
using Opplat.AdminApi.Models;

namespace Opplat.AdminApi.Data;

public static class AdminPortalDataSeeder
{
    private static readonly AdminTenantInfo[] SeedTenants =
    [
        new()
        {
            Id = "tenant-mojocafe",
            Identifier = "mojocafe",
            Name = "MojoCafe",
            DatabaseName = "opplat_mojocafe",
            DatabaseSchema = "tenant_mojocafe",
            UserCount = 18,
            IsActive = true
        },
        new()
        {
            Id = "tenant-bistrocentral",
            Identifier = "bistrocentral",
            Name = "Bistro Central",
            DatabaseName = "opplat_bistrocentral",
            DatabaseSchema = "tenant_bistrocentral",
            UserCount = 9,
            IsActive = true
        },
        new()
        {
            Id = "tenant-lunahub",
            Identifier = "lunahub",
            Name = "Luna Hub",
            DatabaseName = "opplat_lunahub",
            DatabaseSchema = "tenant_lunahub",
            UserCount = 4,
            IsActive = false
        }
    ];

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
        await db.Database.EnsureCreatedAsync(cancellationToken);
        await AdminCatalogSchemaCompatibility.EnsureCurrentSchemaAsync(db, cancellationToken);
        await SeedAsync(db, cancellationToken);
    }

    public static async Task SeedAsync(AdminTenantCatalogDbContext db, CancellationToken cancellationToken = default)
    {
        foreach (var tenant in SeedTenants)
        {
            if (!await db.Tenants.AnyAsync(existing => existing.Id == tenant.Id, cancellationToken))
            {
                db.Tenants.Add(new AdminTenantInfo
                {
                    Id = tenant.Id,
                    Identifier = tenant.Identifier,
                    Name = tenant.Name,
                    DatabaseName = tenant.DatabaseName,
                    DatabaseSchema = tenant.DatabaseSchema,
                    UserCount = tenant.UserCount,
                    IsActive = tenant.IsActive
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
