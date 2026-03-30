// using MediatR;
// using Opplat.Application.Utils;
// using Opplat.Domain.ViewModels;

// namespace Opplat.Application.Features.License.Commands;

// public record AddLicenseCommand(IFormFile Licence) : IRequest<AddLicenseResult>;

// public record AddLicenseResult(bool Success, LicenciaVm? Licencia, string? ErrorMessage);

// public class AddLicenseCommandHandler : IRequestHandler<AddLicenseCommand, AddLicenseResult>
// {
//     private readonly LicenciaService _licenciaService;

//     public AddLicenseCommandHandler(LicenciaService licenciaService)
//     {
//         _licenciaService = licenciaService;
//     }

//     public async Task<AddLicenseResult> Handle(AddLicenseCommand request, CancellationToken cancellationToken)
//     {
//         var response = await _licenciaService.AddLicencia(request.Licence);

//         if (!response.Status)
//             return new AddLicenseResult(false, null, response.Mensaje);

//         var vm = new LicenciaVm
//         {
//             Subscriptor       = response.Licencia!.Subscriptor,
//             FechaVencimiento  = string.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento)
//         };

//         return new AddLicenseResult(true, vm, null);
//     }
// }
