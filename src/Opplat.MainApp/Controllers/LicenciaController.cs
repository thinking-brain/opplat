using LicenceChecker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;
using Opplat.MainApp.Utils;
using Opplat.MainApp.ViewModels;

namespace Opplat.MainApp.Controllers;

[Route("admin/[controller]")]
[ApiController]
public class LicenciaController : Controller
{
    IWebHostEnvironment _enviroment;
    DbContext _db;
    LicenciaService _licenciaService;
    public LicenciaController(IWebHostEnvironment enviroment, OpplatDbContext context, LicenciaService licenciaService)
    {
        _enviroment = enviroment;
        _db = context;
        _licenciaService = licenciaService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await _licenciaService.GetLicencia();
        if (!response.Status)
        {
            return BadRequest(response.Mensaje);
        }
        var result = new LicenciaVm
        {
            Subscriptor = response.Licencia!.Subscriptor,
            FechaVencimiento = String.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento),
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm]IFormFile licence)
    {
        var response = await _licenciaService.AddLicencia(licence);
        if(response.Status)
        {
            return Ok(new LicenciaVm { 
                Subscriptor = response.Licencia!.Subscriptor, 
                FechaVencimiento = String.Format("{0:dd/MM/yyy}", response.Licencia!.Vencimiento)
            });
        }
        return BadRequest(response.Mensaje);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var result = await _licenciaService.Eliminar();
        if (result)
        {
            return Ok("Licencia borrada correctamente.");
        }
        return BadRequest("Error eliminando la licencia, contacte al administrador.");
    }
}
