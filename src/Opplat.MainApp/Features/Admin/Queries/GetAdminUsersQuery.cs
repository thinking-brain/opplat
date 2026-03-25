using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;
using Finbuckle.MultiTenant.Abstractions;

namespace Opplat.MainApp.Features.Admin.Queries;

public record GetAdminUsersQuery(string? TenantIdentifier) : IRequest<List<AdminUserDto>>;

public sealed class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<AdminUserDto>>
{
    private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetAdminUsersQueryHandler> _logger;

    public GetAdminUsersQueryHandler(
        IMultiTenantStore<AppTenantInfo> tenantStore,
        IConfiguration configuration,
        ILogger<GetAdminUsersQueryHandler> logger)
    {
        _tenantStore = tenantStore;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var tenants = (await _tenantStore.GetAllAsync())
            .Where(tenant => tenant.IsActive)
            .Where(tenant => string.IsNullOrWhiteSpace(request.TenantIdentifier) ||
                             string.Equals(tenant.Identifier, request.TenantIdentifier, StringComparison.OrdinalIgnoreCase))
            .Where(tenant => !string.IsNullOrWhiteSpace(tenant.ConnectionString) || !string.IsNullOrWhiteSpace(tenant.DatabaseName))
            .OrderBy(tenant => tenant.Name)
            .ToList();

        var users = new List<AdminUserDto>();
        var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? _configuration.GetConnectionString("MainConnection")
            ?? throw new InvalidOperationException("A default tenant connection string must be configured.");
        foreach (var tenant in tenants)
        {
            try
            {
                await using var db = new OpplatDbContext(
                    new DbContextOptionsBuilder<OpplatDbContext>()
                        .UseNpgsql(PostgresTenantConnectionStringResolver.Resolve(tenant, defaultConnectionString))
                        .Options);

                var tenantUsers = await db.Users
                    .AsNoTracking()
                    .Select(user => new
                    {
                        user.Id,
                        user.Nombres,
                        user.Apellidos,
                        user.UserName,
                        user.Email,
                        user.Activo
                    })
                    .ToListAsync(cancellationToken);

                var roleLookup = await (from userRole in db.Set<IdentityUserRole<string>>().AsNoTracking()
                                        join role in db.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                                        select new { userRole.UserId, role.Name })
                    .ToListAsync(cancellationToken);

                var groupedRoles = roleLookup
                    .Where(role => !string.IsNullOrWhiteSpace(role.Name))
                    .GroupBy(role => role.UserId, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(role => role.Name!).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                        StringComparer.OrdinalIgnoreCase);

                users.AddRange(tenantUsers.Select(user => new AdminUserDto
                {
                    TenantId = tenant.Id ?? string.Empty,
                    TenantIdentifier = tenant.Identifier ?? string.Empty,
                    TenantName = tenant.Name ?? string.Empty,
                    UserId = user.Id,
                    Name = user.Nombres,
                    LastName = user.Apellidos,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Active = user.Activo,
                    Roles = groupedRoles.TryGetValue(user.Id, out var roles) ? roles : new List<string>()
                }));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(
                    ex,
                    "Skipping tenant {TenantIdentifier} during admin user bootstrap because its database could not be queried.",
                    tenant.Identifier);
            }
        }

        return users
            .OrderBy(user => user.TenantName)
            .ThenBy(user => user.Username)
            .ToList();
    }
}
