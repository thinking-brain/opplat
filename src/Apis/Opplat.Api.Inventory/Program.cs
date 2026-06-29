using Opplat.Application.DependencyInjection;
using Opplat.Api.Inventory.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatApplication();

var app = builder.Build();

app.MapInventoryEndpoints();

app.Run();