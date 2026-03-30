using Opplat.AdminApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAdminApi();

var app = builder.Build();

await app.ConfigureAdminApp();

await app.RunAsync();

