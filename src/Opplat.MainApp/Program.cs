using System.Reflection;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Opplat.MainApp.Hosting;
using Opplat.MainApp.Middleware;
using Opplat.Application.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Finbuckle.MultiTenant.Extensions;
using Opplat.Domain.Models;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Opplat.Application.Services;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.MainApp;
using Opplat.Infrastructure.Services;
using Opplat.Application.Utils;
using Opplat.MainApp.Endpoints;
using Opplat.MainApp.Endpoints.Inventory;
using Opplat.MainApp.Endpoints.Sales;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Auth;

var builder = WebApplication.CreateBuilder(args);
var authSection = builder.Configuration.GetSection(AuthOptions.SectionName);
var authOptions = authSection.Get<AuthOptions>() ?? new AuthOptions();
var requireHttpsMetadata = !builder.Environment.IsDevelopment();

// ============================================
// MULTI-TENANT CONFIGURATION
// ============================================
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithRouteStrategy("__tenant__", true)
    .WithHeaderStrategy("X-Tenant-Identifier")
    .WithStore<TenantCatalogStore>(ServiceLifetime.Singleton);

// Add services to the container.
builder.Services.AddDbContext<OpplatDbContext>((serviceProvider, options) =>
{
    var tenantAccessor = serviceProvider.GetService<IMultiTenantContextAccessor<AppTenantInfo>>();
    var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration.GetConnectionString("MainConnection")
        ?? throw new InvalidOperationException("A default or tenant connection string must be configured.");
    var connectionString = PostgresTenantConnectionStringResolver.Resolve(
        tenantAccessor?.MultiTenantContext?.TenantInfo,
        defaultConnectionString);

    options.UseNpgsql(connectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<DbContext, OpplatDbContext>();
builder.Services.Configure<AuthOptions>(authSection);
builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, OidcClaimsTransformation>();

// ============================================
// APP UTILITIES
// ============================================
builder.Services.AddScoped<LicenseService>();
builder.Services.AddScoped<MenuLoader>();
builder.Services.AddScoped<TenantProvisioningService>();

// ============================================
// MEDIATR
// ============================================
builder.Services.AddOpplatApplication();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = requireHttpsMetadata;
        options.MapInboundClaims = false;
        options.Authority = authOptions.Authority;
        if (!string.IsNullOrWhiteSpace(authOptions.MetadataAddress))
            options.MetadataAddress = authOptions.MetadataAddress;
        options.Audience = authOptions.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidIssuer = authOptions.Authority,
            NameClaimType = AuthClaimTypes.PreferredUserName,
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole(authOptions.AdminRole));
    options.AddPolicy("TenantAdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole(authOptions.TenantAdminRole));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var docVersion = builder.Configuration["Documentation:Version"] ?? "1.0.0";
    var docTitle = builder.Configuration["Documentation:Title"] ?? "Opplat API";
    var docDescription = builder.Configuration["Documentation:Description"] ?? "";
    var termsUrl = builder.Configuration["Documentation:TermUrl"] ?? "https://example.com/terms";
    var contactName = builder.Configuration["Documentation:ContactName"] ?? "Support";
    var contactEmail = builder.Configuration["Documentation:ContactEmail"] ?? "support@example.com";
    var contactUrl = builder.Configuration["Documentation:ContactUrl"] ?? "https://example.com/contact";

    c.SwaggerDoc(docVersion, new OpenApiInfo
    {
        Version = docVersion,
        Title = docTitle,
        Description = docDescription,
        TermsOfService = new Uri(termsUrl),
        Contact = new OpenApiContact
        {
            Name = contactName,
            Email = contactEmail,
            Url = new Uri(contactUrl)
        }
    });
    c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter a bearer access token issued by the configured OIDC provider.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
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
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddSignalR();
builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    await ProvisionDevelopmentTenantsAsync(app);

app.UseOpplatAspireDevelopmentSupport();

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

app.UseHttpsRedirectionIfConfigured();
app.UseCors("CorsPolicy");
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

// ============================================
// MINIMAL API ENDPOINTS (Admin / Account / Inventory / License / Menus / Sales)
// ============================================
app.MapOpplatHealthEndpoints("main-api");
app.MapAdminEndpoints();
app.MapAccountEndpoints();
app.MapInventoryEndpoints();
app.MapLicenseEndpoints();
app.MapMenusEndpoints();
app.MapSalesEndpoints();

// app.MapFallbackToFile("index.html");

app.Run();

static async Task ProvisionDevelopmentTenantsAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
    var tenantProvisioningService = scope.ServiceProvider.GetRequiredService<TenantProvisioningService>();
    var tenants = await tenantStore.GetAllAsync();

    foreach (var tenant in tenants.Where(tenant => tenant.IsActive))
        await tenantProvisioningService.ProvisionTenantAsync(tenant);
}
