using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Admin.Commands;

public record SetUserActiveStatusCommand(string UserId, bool Active) : IRequest<bool>;

public sealed class SetUserActiveStatusCommandHandler : IRequestHandler<SetUserActiveStatusCommand, bool>
{
    private readonly OpplatDbContext _db;

    public SetUserActiveStatusCommandHandler(OpplatDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(existing => existing.Id == request.UserId, cancellationToken);
        if (user is null)
            return false;

        user.Activo = request.Active;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
