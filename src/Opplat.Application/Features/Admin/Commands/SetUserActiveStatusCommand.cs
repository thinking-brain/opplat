using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Admin.Commands;

public record SetUserActiveStatusCommand(Guid UserId, bool Active) : IRequest<bool>;

public sealed class SetUserActiveStatusCommandHandler(OpplatDbContext db) : IRequestHandler<SetUserActiveStatusCommand, bool>
{
    private readonly OpplatDbContext _db = db;

    public async Task<bool> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(existing => existing.Id == request.UserId, cancellationToken);
        if (user is null)
            return false;

        user.IsActive = request.Active;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
