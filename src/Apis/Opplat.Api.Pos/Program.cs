using Opplat.Application.DependencyInjection;
using Opplat.Api.Pos.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpplatApplication();

var app = builder.Build();

app.MapPosEndpoints();

app.Run();
