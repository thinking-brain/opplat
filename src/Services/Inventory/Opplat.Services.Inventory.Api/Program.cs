using Opplat.Microservices.Shared.Extensions;
using Opplat.Services.Inventory.Api.Data;
using Opplat.Services.Inventory.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatMicroserviceHost<InventoryDbContext>(builder.Configuration);
builder.Services.AddInventoryModuleServices();

var app = builder.Build();

app.UseOpplatMicroserviceHost();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "inventory" }));

app.Run();
