using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Infrastructure.Services;

namespace Opplat.MainApp.Features.License.Commands;

public record DeleteLicenseCommand : IRequest<bool>;

public class DeleteLicenseCommandHandler(LicenseService licenseService, ILogger<DeleteLicenseCommandHandler> logger) : IRequestHandler<DeleteLicenseCommand, bool>
{
    private readonly LicenseService _licenseService = licenseService;
    private readonly ILogger<DeleteLicenseCommandHandler> _logger = logger;

    public async Task<bool> Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var result = await _licenseService.Delete();
        if (result)
            _logger.LogInformation("License deleted successfully.");
        return result;
    }
}
