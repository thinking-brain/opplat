using MediatR;
using Opplat.Domain.ViewModels;
using Opplat.Infrastructure.Services;

namespace Opplat.Application.Features.License.Queries;

public record GetLicenseQuery : IRequest<GetLicenseResult>;

public record GetLicenseResult(bool Success, LicenciaVm? Licencia, string? ErrorMessage);

public class GetLicenseQueryHandler(LicenciaService licenciaService) : IRequestHandler<GetLicenseQuery, GetLicenseResult>
{
    private readonly LicenciaService _licenciaService = licenciaService;

    public async Task<GetLicenseResult> Handle(GetLicenseQuery request, CancellationToken cancellationToken)
    {
        var response = await _licenciaService.GetLicencia();

        if (!response.Status)
            return new GetLicenseResult(false, null, response.Mensaje);

        var vm = new LicenciaVm
        {
            Subscriptor       = response.Licencia!.Subscriptor,
            FechaVencimiento  = string.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento)
        };

        return new GetLicenseResult(true, vm, null);
    }
}
