using System.Reflection;
using MediatR;

namespace Opplat.Api.Admin.Extensions;

public static class AdminMediatRExtensions
{
    public static IServiceCollection AddAdminMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Selectively register only admin- and account-relevant MediatR handlers from Opplat.Application
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
        var adminHandlerTypes = appAssembly.ExportedTypes
            .Where(t => !t.IsAbstract && !t.IsInterface
                && !adminApiExcludedHandlers.Contains(t.Name)
                && (t.Namespace?.StartsWith("Opplat.Application.Features.Admin") == true
                    || t.Namespace?.StartsWith("Opplat.Application.Features.Account") == true))
            .ToList();

        foreach (var handlerType in adminHandlerTypes)
        {
            foreach (var iface in handlerType.GetInterfaces()
                .Where(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                        || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
                        || i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))))
            {
                services.AddTransient(iface, handlerType);
            }
        }

        return services;
    }
}
