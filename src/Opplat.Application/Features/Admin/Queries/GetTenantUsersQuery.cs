using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Dtos;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Admin.Queries;

public record GetTenantUsersQuery() : IRequest<List<AdminUserDto>>;

public sealed class GetTenantUsersQueryHandler : IRequestHandler<GetTenantUsersQuery, List<AdminUserDto>>
{
    private readonly OpplatDbContext _db;
    // private readonly UserManager<Usuario> _userManager;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;

    public GetTenantUsersQueryHandler(
        OpplatDbContext db,
        // UserManager<Usuario> userManager,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        _db = db;
        // _userManager = userManager;
        _tenantAccessor = tenantAccessor;
    }

    public async Task<List<AdminUserDto>> Handle(GetTenantUsersQuery request, CancellationToken cancellationToken)
    {
        var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo;
        var users = await _db.Users.AsNoTracking().ToListAsync(cancellationToken);
        var response = new List<AdminUserDto>(users.Count);

        foreach (var user in users)
        {
            // var roles = await _userManager.GetRolesAsync(user);
            response.Add(new AdminUserDto
            {
                TenantId = tenant?.Id ?? string.Empty,
                TenantIdentifier = tenant?.Identifier ?? string.Empty,
                TenantName = tenant?.Name ?? string.Empty,
                UserId = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Active = user.IsActive,
                Roles = [] //roles.ToList()
            });
        }

        return response.OrderBy(user => user.Username).ToList();
    }
}
