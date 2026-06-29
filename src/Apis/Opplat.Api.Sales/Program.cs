using Opplat.Application.DependencyInjection;
using Opplat.Api.Sales.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatApplication();

var app = builder.Build();

app.MapSalesEndpoints();

app.Run();