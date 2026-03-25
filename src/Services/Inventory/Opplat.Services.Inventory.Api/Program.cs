using System.Reflection;
using Opplat.Application.DependencyInjection;
using Opplat.Microservices.Shared.Extensions;
using Opplat.Services.Inventory.Api.Data;
using Opplat.Services.Inventory.Api.Endpoints;
using Opplat.Services.Inventory.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);
builder.Services.AddOpplatMicroserviceHost<InventoryDbContext>(builder.Configuration);
builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseOpplatMicroserviceHost();

app.MapOpplatHealthEndpoints("inventory-api");
app.MapInventoryEndpoints();

app.Run();
