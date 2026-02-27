using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Dtos;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Queries;

public record GetUsersQuery : IRequest<List<AccountDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<AccountDto>>
{
    private readonly OpplatDbContext _db;
    private readonly UserManager<Usuario> _userManager;

    public GetUsersQueryHandler(OpplatDbContext db, UserManager<Usuario> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    public async Task<List<AccountDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _db.Set<Usuario>()
            .Select(u => new AccountDto
            {
                UserId   = u.Id,
                Name     = u.Nombres,
                LastName = u.Apellidos,
                Username = u.UserName!,
                Email    = u.Email!,
                Active   = u.Activo,
                Roles    = _userManager.GetRolesAsync(u).Result.ToList()
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}
