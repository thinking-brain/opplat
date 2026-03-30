using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Account.Commands;

public record EditUserCommand(string Id, string Name, string LastName) : IRequest<bool>;

public class EditUserCommandHandler : IRequestHandler<EditUserCommand, bool>
{
    private readonly OpplatDbContext _db;
    private readonly ILogger<EditUserCommandHandler> _logger;

    public EditUserCommandHandler(OpplatDbContext db, ILogger<EditUserCommandHandler> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<bool> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Set<Usuario>().FindAsync([request.Id], cancellationToken);
        if (user == null) return false;

        user.Nombres   = request.Name;
        user.Apellidos = request.LastName;

        var saved = await _db.SaveChangesAsync(cancellationToken);
        if (saved == 1)
            _logger.LogInformation("Usuario {Id} modificado correctamente.", request.Id);

        return saved == 1;
    }
}
