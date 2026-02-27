using MediatR;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record ToggleUserActiveCommand(string UserId) : IRequest<bool>;

public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, bool>
{
    private readonly OpplatDbContext _db;
    private readonly ILogger<ToggleUserActiveCommandHandler> _logger;

    public ToggleUserActiveCommandHandler(OpplatDbContext db, ILogger<ToggleUserActiveCommandHandler> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Usuario>().FindAsync(new object[] { request.UserId }, cancellationToken);
        if (usuario == null) return false;

        usuario.Activo = !usuario.Activo;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Se cambio el estado del usuario {UserName} a {State}.",
            usuario.UserName, usuario.Activo);
        return true;
    }
}
