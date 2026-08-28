using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Opplat.Api.Common.Hosting;
using Opplat.Api.Main.Middleware;
using Opplat.Application.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Finbuckle.MultiTenant.Extensions;
using Opplat.Domain.Models;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Opplat.Application.Services;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Api.Main;
using Opplat.Infrastructure.Services;
using Opplat.Application.Utils;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Auth;
using Npgsql;
using Opplat.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;
using Opplat.Api.Main.Extensions;
using Opplat.Api.Admin.Extensions;
using Opplat.Api.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);
var authSection = builder.Configuration.GetSection(AuthOptions.SectionName);
var authOptions = authSection.Get<AuthOptions>() ?? new AuthOptions();
var requireHttpsMetadata = !builder.Environment.IsDevelopment();

// ============================================
// MULTI-TENANT CONFIGURATION
// ============================================
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHeaderStrategy("X-Tenant-Identifier")
    .WithStore<TenantCatalogStore>(ServiceLifetime.Singleton);

// Add services to the container.
builder.Services.AddDbContext<OpplatDbContext>((serviceProvider, options) =>
{
    var tenantAccessor = serviceProvider.GetService<IMultiTenantContextAccessor<AppTenantInfo>>();
    var tenantDbOptions = builder.Configuration.GetSection(TenantDatabaseOptions.SectionName).Get<TenantDatabaseOptions>()
        ?? new TenantDatabaseOptions();
    var defaultConnectionString = new NpgsqlConnectionStringBuilder
    {
        Host = tenantDbOptions.Host,
        Port = tenantDbOptions.Port,
        Username = tenantDbOptions.Username,
        Password = tenantDbOptions.Password,
        Database = "postgres",
        SslMode = SslMode.Disable
    }.ConnectionString;
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
// MEDIATOR
// ============================================
builder.Services.AddOpplatApplication();
builder.Services.AddAdminMediator();

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

builder.Services.AddOpenApi(options =>
{
    var docVersion = builder.Configuration["Documentation:Version"] ?? "1.0.0";
    var docTitle = builder.Configuration["Documentation:Title"] ?? "Opplat API";
    var docDescription = builder.Configuration["Documentation:Description"] ?? "";
    var termsUrl = builder.Configuration["Documentation:TermUrl"] ?? "https://example.com/terms";
    var contactName = builder.Configuration["Documentation:ContactName"] ?? "Support";
    var contactEmail = builder.Configuration["Documentation:ContactEmail"] ?? "support@example.com";
    var contactUrl = builder.Configuration["Documentation:ContactUrl"] ?? "https://example.com/contact";

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Version = docVersion;
        document.Info.Title = docTitle;
        document.Info.Description = docDescription;
        document.Info.TermsOfService = new Uri(termsUrl);
        document.Info.Contact = new OpenApiContact
        {
            Name = contactName,
            Email = contactEmail,
            Url = new Uri(contactUrl)
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter a bearer access token issued by the configured OIDC provider.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddCorsConfig([
    "http://localhost:3200",
    "http://127.0.0.1:3200",
    "http://localhost:3201",
    "http://127.0.0.1:3201"
]);

builder.Services.AddSignalR();
builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);
builder.Services.AddAdminDatabase(builder.Configuration);
builder.Services.AddAdminInfrastructure(builder.Configuration);
builder.Services.AddInvoicingInfrastructure(builder.Configuration);

var app = builder.Build();

await InitializeAdminCatalogAsync(app);

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
app.MapOpenApi();
app.MapGet("/docs/", () => Results.Redirect("/docs"));
app.MapScalarApiReference("/docs", options =>
{
    options.Title = "Opplat API";
    options.OpenApiRoutePattern = "/openapi/v1.json";
});

app.UseAuthentication();
app.UseMiddleware<TenantValidationMiddleware>();
app.UseAuthorization();

// ============================================
// MULTI-TENANT AWARE ROUTING
// ============================================

app.MapEndpoints();

// app.MapFallbackToFile("index.html");

app.Run();

static async Task InitializeAdminCatalogAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var adminDb = scope.ServiceProvider.GetRequiredService<AdminTenantCatalogDbContext>();

    try
    {
        await adminDb.Database.MigrateAsync();
        logger.LogInformation("Admin catalog database migrations applied successfully");

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Seeding default catalog data");
            await DataSeeder.SeedAsync(adminDb);
            logger.LogInformation("Default catalog data seeded successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize the admin catalog database");
        throw;
    }
}

static async Task ProvisionDevelopmentTenantsAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
    var tenantProvisioningService = scope.ServiceProvider.GetRequiredService<TenantProvisioningService>();
    var tenants = await tenantStore.GetAllAsync();

    foreach (var tenant in tenants.Where(tenant => tenant.IsActive))
        await tenantProvisioningService.ProvisionTenantAsync(tenant);
}
