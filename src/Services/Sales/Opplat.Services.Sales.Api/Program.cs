using System.Reflection;
using Opplat.Application.DependencyInjection;
using Opplat.Microservices.Shared.Extensions;
using Opplat.Services.Sales.Api.Data;
using Opplat.Services.Sales.Api.Endpoints;
using Opplat.Services.Sales.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);
builder.Services.AddOpplatMicroserviceHost<SalesDbContext>(builder.Configuration);
builder.Services.AddOpplatApplication(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseOpplatMicroserviceHost();

app.MapOpplatHealthEndpoints("sales-api");
app.MapSalesEndpoints();

app.Run();
