// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Web;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Opplat.Contabilidad.Domain.Entities;
// using Opplat.Contabilidad.Domain.Services;
// using Opplat.MainApp.Areas.Caja.ViewModels;

// namespace Opplat.MainApp.Areas.Caja.Controllers;

// [Authorize]
// public class OperacionesController : ControllerBase
// {
//     private ICajaService _cajaService;
//     private ICuentaService _cuentaService;

//     public OperacionesController(ICajaService cajaService, ICuentaService cuentaService)
//     {
//         _cajaService = cajaService;
//         _cuentaService = cuentaService;
//     }

//     [HttpPost]
//     public async Task<IEnumerable<ResumenDeOperaciones>> ResumenDeOperaciones(DateTime fecha)
//     {
//         var result = await _cajaService.GetOperaciones(fecha);
//         var operaciones = result.Select(o => new ResumenDeOperaciones
//             {
//                 Tipo = o.Tipo,
//                 Detalle = o.Descripcion,
//                 Importe = o.Importe,
//                 Fecha = o.Fecha,
//                 Usuario = o.Usuario
//             }
//         );
//         return operaciones;
//     }

//     [HttpPost]
//     public async Task<IEnumerable<ResumenDeOperaciones>> ResumenDeVentas(DateTime fecha)
//     {
//         var result = await _cajaService.GetVentas(fecha);
//         var operaciones = result.Select(o => new ResumenDeOperaciones
//             {
//                 Tipo = o.Tipo,
//                 Detalle = o.Descripcion,
//                 Importe = o.Importe,
//                 Fecha = o.Fecha,
//                 Usuario = o.Usuario
//             }
//         );
//         return operaciones;
//     }

//     [HttpGet]
//     public async Task<IEnumerable<CuentaViewModel>> Cuentas()
//     {
//         var cuentas = await _cuentaService.GetCuentas();
//         return cuentas.Select(c => new CuentaViewModel{
//             Id = c.Id,
//             Nombre = c.Nombre
//         });
//     }

//     [HttpPost]
//     public async Task<IActionResult> RealizarAjuste(AsientoContableViewModel asiento)
//     {
//         if (ModelState.IsValid)
//         {
//             string usuario = User.Identity.Name;
//             var result = await _cuentaService.AgregarOperacion(asiento.CuentaCreditoId, asiento.CuentaDebitoId, asiento.Importe,
//                 DateTime.Now, "Ajuste: " + asiento.Observaciones, usuario);
//             if(!result)
//             {
//                 return BadRequest("Error realizando el ajuste");
//             }
//             return Ok("Operacion de ajuste realizada correctamente");
//         }
//         return BadRequest(ModelState.ToString());
//     }

//     [HttpPost]
//     private async Task<IEnumerable<ResumenDeOperaciones>> ResumenDeOperaciones(DateTime fecha, string nombreCuenta)
//     {
//         var operaciones = new List<ResumenDeOperaciones>();
//         var movimientos =
//             await _cuentaService.GetMovimientosDeCuenta(nombreCuenta, fecha);
//         operaciones.AddRange(movimientos.Select(m => new ResumenDeOperaciones()
//         {
//             Fecha = m.Fecha,
//             Importe = m.TipoDeOperacion == TipoDeOperacion.Debito ? m.Importe : -m.Importe,
//             Tipo = m.Descripcion.Substring(0, m.Descripcion.IndexOf(" ")),
//             Detalle = m.Descripcion,
//             Usuario = m.Usuario
//         }));
//         return operaciones;
//     }
// }
