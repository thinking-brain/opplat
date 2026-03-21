using System.Text;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Opplat.Microservices.Shared.Models;

namespace Opplat.Microservices.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpplatMicroserviceHost<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddMultiTenant<AppTenantInfo>()
            .WithRouteStrategy("__tenant__")
            .WithHeaderStrategy("X-Tenant-Identifier")
            .WithConfigurationStore();

        services.AddDbContext<TDbContext>((serviceProvider, options) =>
        {
            var tenantAccessor = serviceProvider.GetService<IMultiTenantContextAccessor<AppTenantInfo>>();
            var connectionString = tenantAccessor?.MultiTenantContext?.TenantInfo?.ConnectionString
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetConnectionString("MainConnection")
                ?? throw new InvalidOperationException("A default or tenant connection string must be configured.");

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<DbContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var signingKey = configuration["Authorization:Password"]
                    ?? throw new InvalidOperationException("Authorization:Password must be configured.");

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["Authorization:Audience"],
                    ValidIssuer = configuration["Authorization:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
            });

        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var version = configuration["Documentation:Version"] ?? "v1";
            var title = configuration["Documentation:Title"] ?? "Opplat Service";
            var description = configuration["Documentation:Description"] ?? "Opplat microservice API";

            options.SwaggerDoc(version, new OpenApiInfo
            {
                Version = version,
                Title = title,
                Description = description,
                TermsOfService = BuildUri(configuration["Documentation:TermUrl"]),
                Contact = new OpenApiContact
                {
                    Name = configuration["Documentation:ContactName"],
                    Email = configuration["Documentation:ContactEmail"],
                    Url = BuildUri(configuration["Documentation:ContactUrl"])
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    []
                }
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.AddControllers();

        return services;
    }

    private static Uri? BuildUri(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
    }
}
