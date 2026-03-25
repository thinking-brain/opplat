using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Opplat.MainApp.Auth;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Services;

public class TenantProvisioningService
{
    private readonly IServiceProvider _serviceProvider;

    public TenantProvisioningService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ProvisionTenantAsync(AppTenantInfo tenantInfo)
    {
        using var scope = _serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("MainConnection")
            ?? throw new InvalidOperationException("A default tenant connection string must be configured.");
        var tenantConnectionString = PostgresTenantConnectionStringResolver.Resolve(
            tenantInfo,
            defaultConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<OpplatDbContext>();
        optionsBuilder.UseNpgsql(tenantConnectionString);

        await EnsureSchemaExistsAsync(tenantConnectionString, tenantInfo.DatabaseSchema);

        await using var context = new OpplatDbContext(optionsBuilder.Options, null);

        await context.Database.EnsureCreatedAsync();

        foreach (var roleName in AuthRoles.TenantAssignable)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == roleName))
            {
                context.Roles.Add(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
            }
        }

        await context.SaveChangesAsync();

        if (!await context.Users.AnyAsync(u => u.UserName == "admin"))
        {
            var tenantAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == AuthRoles.TenantAdmin);
            var tenantUserRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == AuthRoles.TenantUser);
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = $"admin@{tenantInfo.Identifier}.local",
                NormalizedEmail = $"ADMIN@{tenantInfo.Identifier?.ToUpperInvariant()}.LOCAL",
                PasswordHash = "AQAAAAEAACcQAAAAEP4OedI6m26WUn/2C4AcBkzdT6SnL/6E+xakQ/9mGAkqqp3t9PwyIR6l9obLouKIVg==",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Activo = true,
                Nombres = "Tenant",
                Apellidos = tenantInfo.Name ?? "Tenant"
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            var tenantRoles = new[] { tenantAdminRole, tenantUserRole }
                .Where(role => role is not null)
                .Select(role => role!)
                .ToList();

            if (tenantRoles.Count > 0)
            {
                foreach (var role in tenantRoles)
                {
                    context.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = adminUser.Id,
                        RoleId = role.Id
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task EnsureSchemaExistsAsync(string connectionString, string? databaseSchema)
    {
        if (string.IsNullOrWhiteSpace(databaseSchema))
            return;

        var escapedSchema = databaseSchema.Trim().Replace("\"", "\"\"");

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE SCHEMA IF NOT EXISTS \"{escapedSchema}\";";
        await command.ExecuteNonQueryAsync();
    }
}
