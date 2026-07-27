using System.Reflection;
using Opplat.Application.Abstractions.Messaging;

namespace Opplat.Api.Admin.Extensions;

public static class AdminMediatorExtensions
{
    public static IServiceCollection AddAdminMediator(this IServiceCollection services)
    {
        services.AddMediator(Assembly.GetExecutingAssembly());

        // Selectively register only admin- and account-relevant handlers from Opplat.Application
        // to avoid pulling in Sales/Inventory/License/Menus handlers that require unregistered repositories.
        // Handlers that depend on per-tenant services (OpplatDbContext, Finbuckle IMultiTenantContextAccessor /
        // IMultiTenantStore) are excluded — AdminApi does not run in a per-tenant context.
        var adminApiExcludedHandlers = new HashSet<string>(StringComparer.Ordinal)
        {
            "GetAdminUsersQueryHandler",        // uses IMultiTenantStore (Finbuckle, not in AdminApi)
            "GetTenantUsersQueryHandler",       // uses OpplatDbContext + IMultiTenantContextAccessor
            "SetUserActiveStatusCommandHandler",// uses OpplatDbContext (per-tenant)
            "CreateTenantUserCommandHandler",   // uses IMultiTenantContextAccessor (per-tenant)
            "EditUserCommandHandler",           // uses OpplatDbContext (per-tenant account feature)
            "ToggleUserActiveCommandHandler",   // uses OpplatDbContext (per-tenant account feature)
            "GetUsersQueryHandler",             // uses OpplatDbContext (per-tenant account feature)
            "RegisterUserCommandHandler",       // uses OpplatDbContext (per-tenant account feature)
        };

        var appAssembly = typeof(Opplat.Application.Features.Account.Commands.ChangePasswordCommand).Assembly;
        services.AddRequestHandlersFromAssembly(appAssembly, t =>
            !adminApiExcludedHandlers.Contains(t.Name)
            && (t.Namespace?.StartsWith("Opplat.Application.Features.Admin") == true
                || t.Namespace?.StartsWith("Opplat.Application.Features.Account") == true));

        return services;
    }
}
