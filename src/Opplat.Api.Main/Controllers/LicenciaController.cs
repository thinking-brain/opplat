// // LicenciaController has been replaced by Features/License/ (MediatR + Minimal API).
// // Kept for reference only — [ApiController] and [Route] removed.

// // using LicenceChecker;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Opplat.Api.Main.Data;
// using Opplat.Api.Main.Models;
// using Opplat.Api.Main.Utils;
// using Opplat.Api.Main.ViewModels;

// namespace Opplat.Api.Main.Controllers;

// // [Route("admin/[controller]")]
// // [ApiController]
// public class LicenciaController_Archived : Controller
// {
//     IWebHostEnvironment _enviroment;
//     DbContext _db;
//     LicenciaService _licenciaService;
//     public LicenciaController_Archived(IWebHostEnvironment enviroment, OpplatDbContext context, LicenciaService licenciaService)
//     {
//         _enviroment = enviroment;
//         _db = context;
//         _licenciaService = licenciaService;
//     }

//     [HttpGet]
//     public async Task<IActionResult> Get()
//     {
//         var response = await _licenciaService.GetLicencia();
//         if (!response.Status)
//         {
//             return BadRequest(response.Mensaje);
//         }
//         var result = new LicenciaVm
//         {
//             Subscriptor = response.Licencia!.Subscriptor,
//             FechaVencimiento = String.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento),
//         };
//         return Ok(result);
//     }

//     [HttpPost]
//     public async Task<IActionResult> Post([FromForm]IFormFile licence)
//     {
//         var response = await _licenciaService.AddLicencia(licence);
//         if(response.Status)
//         {
//             return Ok(new LicenciaVm { 
//                 Subscriptor = response.Licencia!.Subscriptor, 
//                 FechaVencimiento = String.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento)
//             });
//         }
//         return BadRequest(response.Mensaje);
//     }

//     [HttpDelete]
//     public async Task<IActionResult> Delete()
//     {
//         var result = await _licenciaService.Eliminar();
//         if (result)
//         {
//             return Ok("Licencia borrada correctamente.");
//         }
//         return BadRequest("Error eliminando la licencia, contacte al administrador.");
//     }
// }
