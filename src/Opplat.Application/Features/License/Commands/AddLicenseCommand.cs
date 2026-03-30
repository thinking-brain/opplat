// using MediatR;
// using Opplat.Application.Utils;
// using Opplat.Domain.ViewModels;

// namespace Opplat.Application.Features.License.Commands;

// public record AddLicenseCommand(IFormFile Licence) : IRequest<AddLicenseResult>;

// public record AddLicenseResult(bool Success, LicenseVm? License, string? ErrorMessage);

// public class AddLicenseCommandHandler : IRequestHandler<AddLicenseCommand, AddLicenseResult>
// {
//     private readonly LicenseService _licenseService;

//     public AddLicenseCommandHandler(LicenseService licenseService)
//     {
//         _licenseService = licenseService;
//     }

//     public async Task<AddLicenseResult> Handle(AddLicenseCommand request, CancellationToken cancellationToken)
//     {
//         var response = await _licenseService.AddLicense(request.Licence);

//         if (!response.Status)
//             return new AddLicenseResult(false, null, response.Message);

//         var vm = new LicenseVm
//         {
//             Subscriber      = response.License!.Subscriber,
//             ExpirationDate  = string.Format("{0:dd/MM/yyy}", response.License!.ExpirationDate)
//         };

//         return new AddLicenseResult(true, vm, null);
//     }
// }
