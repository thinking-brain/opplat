using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Infrastructure.Services;

namespace Opplat.MainApp.Features.License.Commands;

public record DeleteLicenseCommand : IRequest<bool>;

public class DeleteLicenseCommandHandler(LicenciaService licenciaService, ILogger<DeleteLicenseCommandHandler> logger) : IRequestHandler<DeleteLicenseCommand, bool>
{
    private readonly LicenciaService _licenciaService = licenciaService;
    private readonly ILogger<DeleteLicenseCommandHandler> _logger = logger;

    public async Task<bool> Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var result = await _licenciaService.Eliminar();
        if (result)
            _logger.LogInformation("Licencia eliminada correctamente.");
        return result;
    }
}
