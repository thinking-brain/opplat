using MediatR;
using Opplat.Application.Dtos;

namespace Opplat.MainApp.Features.Account.Queries;

public record GetUserProfileQuery(string Username) : IRequest<AccountDto?>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, AccountDto?>
{
    // private readonly UserManager<Usuario> _userManager;

    public GetUserProfileQueryHandler(/*UserManager<Usuario> userManager*/)
    {
        // _userManager = userManager;
    }

    public async Task<AccountDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // var user = await _userManager.FindByNameAsync(request.Username);
        // if (user == null) return null;

        // var roles = await _userManager.GetRolesAsync(user);
        // return new AccountDto
        // {
        //     UserId   = user.Id,
        //     Name     = user.Nombres,
        //     LastName = user.Apellidos,
        //     Username = user.UserName!,
        //     Email    = user.Email!,
        //     Active   = user.Activo,
        //     Roles    = roles.ToList()
        // };
        return null;
    }
}
