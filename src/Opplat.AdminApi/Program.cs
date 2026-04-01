using Microsoft.EntityFrameworkCore;
using Opplat.AdminApi.Data;
using Opplat.AdminApi.Extensions;
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
}

// Seed development data if in Development environment
if (app.Environment.IsDevelopment())
{
    await DevDataSeeder.SeedAsync(app.Services, logger);
}

await app.ConfigureAdminApp();

await app.RunAsync();

