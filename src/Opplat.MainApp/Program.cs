using System.Reflection;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Opplat.MainApp.Data;
using Opplat.MainApp.Features.Account;
using Opplat.MainApp.Features.License;
using Opplat.MainApp.Features.Menus;
using Opplat.MainApp.Models;
using Opplat.MainApp.Middleware;
using Opplat.MainApp.Utils;
using SalesServices = Opplat.Modules.Sales.Domain.Services;
using SalesRepositories = Opplat.Modules.Sales.Domain.Repositories;
using InfrastructureSalesRepositories = Opplat.Modules.Sales.Infrastructure.Repositories;
using InventoryServices = Opplat.Modules.Inventory.Domain.Services;
using InventoryRepositories = Opplat.Modules.Inventory.Domain.Repositories;
using InfrastructureInventoryRepositories = Opplat.Modules.Inventory.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// MULTI-TENANT CONFIGURATION
// ============================================
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithRouteStrategy("__tenant__")
    .WithHeaderStrategy("X-Tenant-Identifier")
    .WithConfigurationStore();

// Add services to the container.
builder.Services.AddDbContext<OpplatDbContext>((serviceProvider, options) =>
{
    var tenantAccessor = serviceProvider.GetService<IMultiTenantContextAccessor<AppTenantInfo>>();
    var connectionString = tenantAccessor?.MultiTenantContext?.TenantInfo?.ConnectionString 
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration.GetConnectionString("MainConnection");
    
    options.UseSqlServer(connectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<Usuario, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<OpplatDbContext>()
    .AddDefaultTokenProviders();

// builder.Services.AddIdentityServer()
//     .AddApiAuthorization<Usuario, OpplatDbContext>();

builder.Services.AddScoped<DbContext, OpplatDbContext>();

// ============================================
// APP UTILITIES
// ============================================
builder.Services.AddScoped<LicenciaService>();
builder.Services.AddScoped<MenuLoader>();

// ============================================
// MEDIATR
// ============================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
// Sales DI configurations
builder.Services.AddScoped<SalesServices.IProductService, SalesServices.ProductService>();
builder.Services.AddScoped<SalesRepositories.IProductRepository, InfrastructureSalesRepositories.ProductsRepository>();
builder.Services.AddScoped<SalesServices.IToppingService, SalesServices.ToppingService>();
builder.Services.AddScoped<SalesRepositories.IToppingRepository, InfrastructureSalesRepositories.ToppingRepository>();
builder.Services.AddScoped<SalesServices.IProductTagService, SalesServices.ProductTagService>();
builder.Services.AddScoped<SalesRepositories.IProductTagRepository, InfrastructureSalesRepositories.ProductTagRepository>();
builder.Services.AddScoped<SalesServices.ICostTabService, SalesServices.CostTabService>();
builder.Services.AddScoped<SalesRepositories.ICostTabRepository, InfrastructureSalesRepositories.CostTabRepository>();
// Inventory DI configurations
builder.Services.AddScoped<InventoryServices.IProductClassificationService, InventoryServices.ProductClassificationService>();
builder.Services.AddScoped<InventoryRepositories.IProductClassificationRepository, InfrastructureInventoryRepositories.ProductClassificationRepository>();
builder.Services.AddScoped<InventoryServices.IProductGroupService, InventoryServices.ProductGroupService>();
builder.Services.AddScoped<InventoryRepositories.IProductGroupRepository, InfrastructureInventoryRepositories.ProductGroupRepository>();
builder.Services.AddScoped<InventoryServices.IProductService, InventoryServices.ProductService>();
builder.Services.AddScoped<InventoryRepositories.IProductRepository, InfrastructureInventoryRepositories.ProductsRepository>();
builder.Services.AddScoped<InventoryServices.IStorageService, InventoryServices.StorageService>();
builder.Services.AddScoped<InventoryRepositories.IStorageRepository, InfrastructureInventoryRepositories.StorageRepository>();
builder.Services.AddScoped<InventoryServices.IMovementTypeService, InventoryServices.MovementTypeService>();
builder.Services.AddScoped<InventoryServices.IProductMovementService, InventoryServices.ProductMovementService>();
builder.Services.AddScoped<InventoryRepositories.IMovementsRepository, InfrastructureInventoryRepositories.ProductMovementRepository>();
builder.Services.AddScoped<InventoryServices.IInventoryService, InventoryServices.InventoryService>();
builder.Services.AddScoped<InventoryRepositories.IInventoryRepository, InfrastructureInventoryRepositories.InventoryRepository>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authorization:Audience"],
            ValidIssuer = builder.Configuration["Authorization:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authorization:Password"]))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(builder.Configuration["Documentation:Version"], new OpenApiInfo
    {
        Version = builder.Configuration["Documentation:Version"],
        Title = builder.Configuration["Documentation:Title"],
        Description = builder.Configuration["Documentation:Description"],
        TermsOfService = new Uri(builder.Configuration["Documentation:TermUrl"]),
        Contact = new OpenApiContact
        {
            Name = builder.Configuration["Documentation:ContactName"],
            Email = builder.Configuration["Documentation:ContactEmail"],
            Url = new Uri(builder.Configuration["Documentation:ContactUrl"])
        }
    });
    c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
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
builder.Services.AddControllers();
// builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages();

var app = builder.Build();

// Add multi-tenant middleware EARLY in the pipeline
app.UseMultiTenant();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/docs/v1/docs.json", "Opplat API v1");
    c.RoutePrefix = "docs";
});

app.UseAuthentication();
app.UseMiddleware<TenantValidationMiddleware>();
app.UseAuthorization();

// ============================================
// MULTI-TENANT AWARE ROUTING
// ============================================

// MVC attribute-routing for Sales & Inventory area controllers
app.MapControllers();

// Area convention routes (backward compat)
app.MapAreaControllerRoute(
            name: "SalesArea",
            areaName: "Sales",
            pattern: "Sales/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
            name: "InventoryArea",
            areaName: "inventory",
            pattern: "inventory/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tenant-sales",
    pattern: "{__tenant__}/Sales/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Sales" });

app.MapControllerRoute(
    name: "tenant-inventory",
    pattern: "{__tenant__}/inventory/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "inventory" });

// ============================================
// MINIMAL API ENDPOINTS (Account / License / Menus)
// ============================================
app.MapAccountEndpoints();
app.MapLicenseEndpoints();
app.MapMenusEndpoints();


app.UseCors("CorsPolicy");

// app.MapFallbackToFile("index.html");

app.Run();
