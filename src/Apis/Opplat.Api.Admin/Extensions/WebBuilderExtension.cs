using Opplat.Api.Admin.Auth;
using Opplat.Api.Admin.Hosting;
using Opplat.Application.Abstractions.Options;
using Opplat.Infrastructure.DependencyInjection;

namespace Opplat.Api.Admin.Extensions;

public static class WebBuilderExtension
{
    public static WebApplicationBuilder AddAdminApi(this WebApplicationBuilder builder)
    {
        var authSection = builder.Configuration.GetSection(AuthOptions.SectionName);
        var authOptions = authSection.Get<AuthOptions>() ?? new AuthOptions();
        var authRuntime = AuthRuntimeConfigurationResolver.Resolve(authOptions);
        var adminBffOptions = authOptions.AdminBff;

        var legacyAdminClientId = authSection["ClientIdAdmin"];
        if (!string.IsNullOrWhiteSpace(legacyAdminClientId))
            adminBffOptions.ClientId = legacyAdminClientId.Trim();

        var legacyAdminClientSecret = authSection["ClientSecretAdmin"];
        if (!string.IsNullOrWhiteSpace(legacyAdminClientSecret))
            adminBffOptions.ClientSecret = legacyAdminClientSecret.Trim();

        var allowedCorsOrigins = adminBffOptions.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var requireHttpsMetadata = !builder.Environment.IsDevelopment();

        builder.Services.Configure<AuthOptions>(options =>
        {
            options.Provider = authOptions.Provider;
            options.Authority = authRuntime.Authority;
            options.MetadataAddress = authRuntime.MetadataAddress;
            options.Audience = authRuntime.Audience;
            options.ValidIssuers = [.. authRuntime.ValidIssuers];
            options.ClaimNamespace = authOptions.ClaimNamespace;
            options.AdminRole = authOptions.AdminRole;
            options.TenantAdminRole = authOptions.TenantAdminRole;
            options.TenantUserRole = authOptions.TenantUserRole;
            options.AdditionalRoleClaimTypes = [.. authOptions.AdditionalRoleClaimTypes];
            options.Entra = new EntraAuthOptions
            {
                TenantId = authOptions.Entra.TenantId,
                AuthorityHost = authOptions.Entra.AuthorityHost,
                UseV2Endpoint = authOptions.Entra.UseV2Endpoint,
                ObjectIdClaimType = authOptions.Entra.ObjectIdClaimType
            };
            options.AdminBff = new AdminBffOptions
            {
                ClientId = adminBffOptions.ClientId,
                ClientSecret = adminBffOptions.ClientSecret,
                ShellModeEnabled = adminBffOptions.ShellModeEnabled,
                Scopes = [.. adminBffOptions.Scopes],
                UsePkce = adminBffOptions.UsePkce,
                GetClaimsFromUserInfoEndpoint = adminBffOptions.GetClaimsFromUserInfoEndpoint,
                UseAudienceQueryParam = adminBffOptions.UseAudienceQueryParam,
                LoginPath = adminBffOptions.LoginPath,
                LogoutPath = adminBffOptions.LogoutPath,
                CallbackPath = adminBffOptions.CallbackPath,
                SignedOutCallbackPath = adminBffOptions.SignedOutCallbackPath,
                AccessDeniedPath = adminBffOptions.AccessDeniedPath,
                SessionCookieName = adminBffOptions.SessionCookieName,
                CsrfCookieName = adminBffOptions.CsrfCookieName,
                CsrfHeaderName = adminBffOptions.CsrfHeaderName,
                SessionHours = adminBffOptions.SessionHours,
                DefaultOrigin = adminBffOptions.DefaultOrigin,
                AllowedOrigins = [.. adminBffOptions.AllowedOrigins]
            };
        });

        builder.Services.AddAdminDatabase(builder.Configuration);
        builder.Services.AddAdminMediatR();
        builder.Services.AddAdminAuthentication(authOptions, authRuntime, adminBffOptions, requireHttpsMetadata);
        builder.Services.AddAdminCors(allowedCorsOrigins);
        builder.Services.AddAdminInfrastructure(builder.Configuration);
        builder.Services.AddOpplatAspireDevelopmentSupport(builder.Environment);

        return builder;
    }
}