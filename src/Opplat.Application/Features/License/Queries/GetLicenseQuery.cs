using MediatR;
using Opplat.Domain.ViewModels;
using Opplat.Infrastructure.Services;

namespace Opplat.Application.Features.License.Queries;

public record GetLicenseQuery : IRequest<GetLicenseResult>;

public record GetLicenseResult(bool Success, LicenseVm? License, string? ErrorMessage);

public class GetLicenseQueryHandler(LicenseService licenseService) : IRequestHandler<GetLicenseQuery, GetLicenseResult>
{
    private readonly LicenseService _licenseService = licenseService;

    public async Task<GetLicenseResult> Handle(GetLicenseQuery request, CancellationToken cancellationToken)
    {
        var response = await _licenseService.GetLicense();

        if (!response.Status)
            return new GetLicenseResult(false, null, response.Message);

        var vm = new LicenseVm
        {
            Subscriber      = response.License!.Subscriber,
            ExpirationDate  = string.Format("{0:dd/MM/yyy}", response.License!.ExpirationDate)
        };

        return new GetLicenseResult(true, vm, null);
    }
}
