using MediatR;
using Opplat.MainApp.Utils;

namespace Opplat.MainApp.Features.License.Commands;

public record DeleteLicenseCommand : IRequest<bool>;

public class DeleteLicenseCommandHandler : IRequestHandler<DeleteLicenseCommand, bool>
{
    private readonly LicenciaService _licenciaService;
    private readonly ILogger<DeleteLicenseCommandHandler> _logger;

    public DeleteLicenseCommandHandler(LicenciaService licenciaService, ILogger<DeleteLicenseCommandHandler> logger)
    {
        _licenciaService = licenciaService;
        _logger          = logger;
    }

    public async Task<bool> Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var result = await _licenciaService.Eliminar();
        if (result)
            _logger.LogInformation("Licencia eliminada correctamente.");
        return result;
    }
}
