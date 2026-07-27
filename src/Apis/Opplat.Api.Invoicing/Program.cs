using Opplat.Application.DependencyInjection;
using Opplat.Api.Invoicing.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatApplication();

var app = builder.Build();

app.MapInvoicingEndpoints();

app.Run();