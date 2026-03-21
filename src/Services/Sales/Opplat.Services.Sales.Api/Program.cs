using Opplat.Microservices.Shared.Extensions;
using Opplat.Services.Sales.Api.Data;
using Opplat.Services.Sales.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatMicroserviceHost<SalesDbContext>(builder.Configuration);
builder.Services.AddSalesModuleServices();

var app = builder.Build();

app.UseOpplatMicroserviceHost();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "sales" }));

app.Run();
