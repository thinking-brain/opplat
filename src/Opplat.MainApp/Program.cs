using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;
using Opplat.Domain.Sales.Services;
using Opplat.Domain.Sales.Repositories;
using Opplat.Infrastructure.Sales.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MainConnection");
builder.Services.AddDbContext<OpplatDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<Usuario, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<OpplatDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<Usuario, OpplatDbContext>();

builder.Services.AddScoped<DbContext, OpplatDbContext>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductsRepository>();
builder.Services.AddScoped<IToppingService, ToppingService>();
builder.Services.AddScoped<IToppingRepository, ToppingRepository>();
// builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<IProductRepository, ProductsRepository>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Opplat API",
        Description = "Gestiona la autenticacion y autorizacion, asi como la gestion de usuarios.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "EFAVAI Tech",
            Email = "efavai.tech@gmail.com",
            Url = new Uri("https://efavai.com/")
        }
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "MyHttpHeaderName",
        Description = "My description",
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/docs/v1/docs.json", "Opplat API v1");
    c.RoutePrefix = "docs";
});

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapAreaControllerRoute(
            name: "SalesArea",
            areaName: "Sales",
            pattern: "Sales/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();


app.UseCors("CorsPolicy");

app.MapFallbackToFile("index.html"); ;

app.Run();
