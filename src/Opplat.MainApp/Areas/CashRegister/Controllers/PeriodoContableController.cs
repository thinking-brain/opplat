// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Web;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Opplat.Contabilidad.Domain.Entities;
// using Opplat.Contabilidad.Domain.Services;
// using Opplat.MainApp.Areas.Caja.ViewModels;

// namespace Opplat.MainApp.Areas.Caja.Controllers;

// [Authorize]
// public class PeriodoContableController : Controller
// {
//     private IPeriodoContableService _service;
//     private ICuentaService _cuentasServices;
//     private ICierreService _cierreService;
//     private ICajaService _cajaService;

//     public PeriodoContableController(IPeriodoContableService service, ICuentaService cuentaService, 
//         ICierreService cierreService, ICajaService cajaService)
//     {
//         _service = service;
//         _cuentasServices = cuentaService;
//         _cierreService = cierreService;
//         _cajaService = cajaService;
//     }

//     public async Task<IEnumerable<DiaContable>> DiasContables()
//     {
//         return await _service.GetPeriodosContables();
//     }

//     [HttpGet]
//     public async Task<DiaContable> Get(Guid id)
//     {
//         return await _service.FindById(id);
//     }

//     [HttpPost]
//     public async Task<IActionResult> EditarDia(DiaContable diaContable)
//     {
//         if (ModelState.IsValid)
//         {
//             var result = await _service.Update(diaContable);
//             if (result)
//             {
//                 return Ok("Dia editado correctamente");
//             }
//             return BadRequest("Error al editar el dia");
//         }
//         return BadRequest(ModelState.ToString());
//     }

//     [HttpGet]
//     public async Task<IEnumerable<CierreDeCaja>> Cierres()
//     {
//         var cierres = await _cierreService.GetCierres();
//         return cierres;
//     }
//     // GET: PeriodoContable

//     [HttpGet]
//     public async Task<string> DiaContable()
//     {
//         var diaContable = await _service.GetDiaContableActual();
//         return diaContable.Fecha.ToShortDateString();
//     }

//     public JsonResult ResumenEfectivo()
//     {
//         return ResumenEfectivoData(_service.GetDiaContableActual().Id);
//     }

//     public PartialViewResult DesgloseCierrePartial(int id)
//     {
//         var dia = _service.BuscarDiaContable(id);
//         var cierre = _db.Set<CierreDeCaja>().SingleOrDefault(c => c.DiaContableId == dia.Id);
//         return PartialView("_DesgloseDeEfectivoCierrePartial", cierre.Desglose);
//     }

//     public JsonResult ResumenEfectivoData(int diaId)
//     {
//         var resumenCierre = new ResumenCierre(_db);
//         var resumen = resumenCierre.VerResumen(diaId);
//         return Json(resumen);
//     }

//     [HttpGet]
//     public async Task<DesgloceEfectivoViewModel> Denominaciones()
//     {
//         var denominaciones = await _cajaService.GetDenominaciones();
//         var billetes = denominaciones.Where(d => d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
//         {
//             Valor = d.Key,
//             CantidadCup = 0,
//             CantidadCuc = 0,
//             CantidadCupExtraccion = 0,
//             CantidadCucExtraccion = 0,
//             Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
//             Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
//         });

//         var monedas = denominaciones.Where(d => !d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
//         {
//             Valor = d.Key,
//             CantidadCup = 0,
//             CantidadCuc = 0,
//             CantidadCupExtraccion = 0,
//             CantidadCucExtraccion = 0,
//             Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
//             Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
//         });

//         var desglose = new DesgloceEfectivoViewModel()
//         {
//             Billetes = billetes.ToList(),
//             Monedas = monedas.ToList(),
//         };
//         return desglose;
//     }

//     [HttpPost]
//     public JsonResult CerrarPeriodo([FromBody] DesgloceEfectivoViewModel desgloceEfectivoViewModel)
//     {
//         var importeAExtraer = desgloceEfectivoViewModel.Billetes.Sum(b => b.CantidadCucExtraccion * b.Valor)
//             + (desgloceEfectivoViewModel.Billetes.Sum(b => b.CantidadCupExtraccion * b.Valor) / 24)
//             + (desgloceEfectivoViewModel.Monedas.Sum(b => b.CantidadCucExtraccion * b.Valor))
//             + (desgloceEfectivoViewModel.Monedas.Sum(b => b.CantidadCupExtraccion * b.Valor) / 24);
//         if (importeAExtraer >= 0)
//         {
//             var dia = _service.GetDiaContableActual();
//             var usuario = _db.Set<Usuario>().SingleOrDefault(u => u.UserName == User.Identity.Name);

//             var cuentaCaja = _cuentasServices.FindCuentaByNombre("Caja");
//             var cuentaBanco = _cuentasServices.FindCuentaByNombre("Banco");
//             var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
//             if (cuentaCaja.Disponibilidad.Saldo < importeAExtraer)
//             {
//                 var ganancias = (importeAExtraer) - cuentaGasto.Disponibilidad.Saldo;
//                 cuentaCaja.Disponibilidad.Saldo = cuentaCaja.Disponibilidad.Saldo + ganancias;
//                 return Json(new { result = false, mensaje = "No se puede realizar la extraccion de la caja, saldo en caja inferior al extraer" });
//             }
//             _cuentasServices.AgregarOperacion(cuentaCaja.Id, cuentaBanco.Id, importeAExtraer, DateTime.Now, "Cierre del dia",
//                 usuario.Id);

//             var caja = _db.Set<Caja.Core.Models.Caja>().FirstOrDefault();
//             var cierre = new CierreDeCaja()
//             {
//                 CajaId = caja.Id,
//                 DiaContableId = dia.Id
//             };
//             var cuc = _db.Set<Moneda>().SingleOrDefault(m => m.Sigla == "CUC");
//             var cup = _db.Set<Moneda>().SingleOrDefault(m => m.Sigla == "CUP");
//             foreach (var billete in desgloceEfectivoViewModel.Billetes)
//             {
//                 if (billete.Cuc && billete.CantidadCuc > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cuc.Id && d.Valor == billete.Valor);
//                     cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = billete.CantidadCuc
//                     });
//                 }
//                 if (billete.Cuc && billete.CantidadCucExtraccion > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cuc.Id && d.Valor == billete.Valor);
//                     _db.Set<DenominacionExtraccionCierre>().Add(new DenominacionExtraccionCierre()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = billete.CantidadCucExtraccion,
//                         CierreDeCaja = cierre,
//                     });
//                 }
//                 if (billete.Cup && billete.CantidadCup > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cup.Id && d.Valor == billete.Valor);
//                     cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = billete.CantidadCup
//                     });
//                 }
//                 if (billete.Cup && billete.CantidadCupExtraccion > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cup.Id && d.Valor == billete.Valor);
//                     _db.Set<DenominacionExtraccionCierre>().Add(new DenominacionExtraccionCierre()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = billete.CantidadCupExtraccion,
//                         CierreDeCaja = cierre,
//                     });
//                 }
//             }
//             foreach (var moneda in desgloceEfectivoViewModel.Monedas)
//             {
//                 if (moneda.Cuc && moneda.CantidadCuc > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cuc.Id && d.Valor == moneda.Valor);
//                     cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = moneda.CantidadCuc,
//                     });
//                 }
//                 if (moneda.Cuc && moneda.CantidadCucExtraccion > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cuc.Id && d.Valor == moneda.Valor);
//                     _db.Set<DenominacionExtraccionCierre>().Add(new DenominacionExtraccionCierre()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = moneda.CantidadCucExtraccion,
//                         CierreDeCaja = cierre,
//                     });
//                 }
//                 if (moneda.Cup && moneda.CantidadCup > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cup.Id && d.Valor == moneda.Valor);
//                     cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = moneda.CantidadCup
//                     });
//                 }
//                 if (moneda.Cup && moneda.CantidadCupExtraccion > 0)
//                 {
//                     var denominacion =
//                         _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cup.Id && d.Valor == moneda.Valor);
//                     _db.Set<DenominacionExtraccionCierre>().Add(new DenominacionExtraccionCierre()
//                     {
//                         DenominacionDeMonedaId = denominacion.Id,
//                         Cantidad = moneda.CantidadCupExtraccion,
//                         CierreDeCaja = cierre,
//                     });
//                 }
//             }
//             _cierreService.CerrarCaja(cierre);
//             _service.CerrarDiaContable();
//             _db.SaveChanges();
//             //HttpContext.GetOwinContext().Authentication.SignOut();
//             return Json(new { result = true, cierreId = cierre.DiaContableId, mensaje = "Cierre correcto" });

//         }
//         return Json(new { result = false, mensaje = "No se puede cerrar, importe a extraer negativo" });
//     }

//     [HttpGet]
//     public async Task<IActionResult> AbrirDia()
//     {
//         var dia = await _service.SePuedeAbrirDia();
//         if (dia == null)
//         {
//             var fecha = DateTime.Now;
//             while (_service.BuscarDiaContable(fecha) != null)
//             {
//                 fecha = fecha.AddDays(1);
//             }
//             ViewBag.Fecha = fecha;
//             ViewBag.SePuede = true;
//             return Ok(new {
//                 Fecha = dia.Fecha,
//                 SePuede = true
//             });
//         }
//         else
//         {
//             return Ok(new {
//                 Fecha = dia.Fecha,
//                 SePuede = false
//             });
//         }
//     }

//     [HttpPost]
//     public ActionResult AbrirDia(AbrirDiaViewModel abrirDiaViewModel)
//     {

//         if(abrirDiaViewModel.Fecha.HasValue)
//         {
//             _service.EmpezarDiaContable(abrirDiaViewModel.Fecha.Value);
//         }
//         else
//         {
//             _service.EmpezarDiaContable(DateTime.Now);
//         }
//         return Ok("Dia abierto correctamente");
//     }

//     [HttpGet]
//     public async Task<IActionResult> SePuedeCerrar()
//     {
//         var result = await _cierreService.SePuedeCerrar();
//         return Ok("Se puede cerrar.");
//     }
// }
