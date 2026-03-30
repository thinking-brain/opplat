using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Account.Commands;

public record ToggleUserActiveCommand(string UserId) : IRequest<bool>;

public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, bool>
{
    private readonly OpplatDbContext _db;
    private readonly ILogger<ToggleUserActiveCommandHandler> _logger;

    public ToggleUserActiveCommandHandler(OpplatDbContext db, ILogger<ToggleUserActiveCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Set<User>().FindAsync([request.UserId], cancellationToken);
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Changed the status of user {UserName} to {State}.",
            user.UserName, user.IsActive);
        return true;
    }
}
