using Opplat.Application.DependencyInjection;
using Opplat.Api.Account.Endpoints;
using Opplat.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatApplication();
builder.Services.AddAdminInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapAccountEndpoints();

app.Run();