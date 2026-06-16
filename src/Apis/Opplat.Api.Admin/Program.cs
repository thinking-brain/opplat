using Microsoft.EntityFrameworkCore;
using Opplat.Api.Admin.Extensions;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Persistance.Data.Administration;

var builder = WebApplication.CreateBuilder(args);

builder.AddAdminApi();

var app = builder.Build();

// Apply pending EF Core migrations on startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Applying pending database migrations...");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();
    await db.Database.MigrateAsync();
    logger.LogInformation("Database migrations applied successfully");
    if (app.Environment.IsDevelopment())
    {
        DataSeeder.SeedAsync(db, default).Wait();
    }
}

await app.ConfigureAdminApp();

await app.RunAsync();

