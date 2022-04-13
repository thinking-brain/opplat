using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Opplat.Contabilidad.Domain.Services;
using Opplat.MainApp.Areas.Caja.ViewModels;

namespace Opplat.MainApp.Areas.Caja.Controllers;

[Authorize]
public class CajaController : ControllerBase
{
    private ICajaService _cajaService;

    public CajaController(ICajaService cajaService)
    {
        _cajaService = cajaService;
    }

    [HttpPost]
    public async Task<IActionResult> Extraccion(OperacionCajaViewModel extraccion)
    {
        if (ModelState.IsValid)
        {
            if (extraccion.Importe < 0)
            {
                return BadRequest("No se puede realizar una Extracción con saldo negativo");
            }
            string usuario = User!.Identity!.Name!;
            var result = await _cajaService.Extraer(extraccion.Importe, extraccion.Observaciones, usuario);
            if(!result)
            {
                return BadRequest("Error efectuando la extraccion.");
            }
            return Ok("Extracción efectuada correctamente");
        }
        return BadRequest(ModelState.ToString());
    }

    [HttpPost]
    public async Task<IActionResult> Deposito(OperacionCajaViewModel deposito)
    {
        if (ModelState.IsValid)
        {
            string usuario = User!.Identity!.Name!;
            var result = await _cajaService.Depositar(deposito.Importe, deposito.Observaciones, usuario);
            if(!result)
            {
                return BadRequest("Error efectuando el deposito.");
            }
            return Ok("Deposito efectuado correctamente");
        }
        return BadRequest(ModelState.ToString());
    }

    [HttpPost]
    public async Task<IActionResult> SePuedeExtraer(decimal importe)
    {
        var result = await _cajaService.SePuedeExtraer(importe);
        return Ok(result);
    }
}
