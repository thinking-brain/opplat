using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        
        var optionsBuilder = new DbContextOptionsBuilder<OpplatDbContext>();
        optionsBuilder.UseSqlServer(tenantInfo.ConnectionString);
        
        await using var context = new OpplatDbContext(optionsBuilder.Options, null);
        
        await context.Database.MigrateAsync();
        
        if (!await context.Roles.AnyAsync(r => r.Name == "administrador"))
        {
            context.Roles.Add(new IdentityRole 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = "administrador", 
                NormalizedName = "ADMINISTRADOR" 
            });
            await context.SaveChangesAsync();
        }
        
        if (!await context.Users.AnyAsync(u => u.UserName == "admin"))
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "administrador");
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = $"admin@{tenantInfo.Identifier}.com",
                NormalizedEmail = $"ADMIN@{tenantInfo.Identifier?.ToUpper()}.COM",
                PasswordHash = "AQAAAAEAACcQAAAAEP4OedI6m26WUn/2C4AcBkzdT6SnL/6E+xakQ/9mGAkqqp3t9PwyIR6l9obLouKIVg==",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Activo = true,
                Nombres = "Administrador",
                Apellidos = tenantInfo.Name ?? "Tenant"
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
            
            if (adminRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
