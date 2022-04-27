// using System.Collections.Generic;
// using System.Linq;
// using Caja.Core.Models;
// using ContabilidadBL.Core;
// using Contabilidad.Core.Models;
// using CajaModule.ViewModels;
// using Microsoft.EntityFrameworkCore;
// using VentaProducto.Core.Models;
// using Commons.Core.Extentions;
// using System;
// using Modulos.Core;

// namespace Opplat.MainApp.Areas.Utils
// {
//     public class ResumenCierre:ICierreContableLoader
//     {
//         private DbContext _db;
//         private CuentasServices _cuentasServices;
//         private PeriodoContableService _periodoContableService;

//         public ResumenCierre(DbContext context)
//         {
//             _db = context;
//             _cuentasServices = new CuentasServices(context);
//             _periodoContableService = new PeriodoContableService(context);
//         }

//         public CierreViewModel VerResumen(int id)
//         {

//             var dia = _db.Set<DiaContable>().FirstOrDefault(d => d.Id == id);
//             var cierreCaja = _db.Set<CierreDeCaja>()
//                 .Include(c => c.Desglose).ThenInclude(c => c.DenominacionDeMoneda.Moneda) 
//                 .SingleOrDefault(c => c.DiaContableId == dia.Id);
//             var efectivo = _cuentasServices.FindCuentaByNombre("Caja").Disponibilidad.Saldo;
//             var diaAnterior = _db.Set<DiaContable>().Where(d => d.Fecha < dia.Fecha).OrderByDescending(d => d.Fecha).FirstOrDefault();
//             decimal importeAnterior = 0;
//             if (diaAnterior != null)
//             {
//                 var denominaciones = _db.Set<DenominacionesEnCierreDeCaja>()
//                     .Include(d => d.DenominacionDeMoneda.Moneda)
//                     .Include(d => d.CierreDeCaja)
//                     .Where(d => d.CierreDeCaja.DiaContableId == diaAnterior.Id);
//                 foreach (var denominacion in denominaciones)
//                 {
//                     if (denominacion.DenominacionDeMoneda.Moneda.Sigla == "CUC")
//                     {
//                         importeAnterior += denominacion.Cantidad * denominacion.DenominacionDeMoneda.Valor;
//                     }
//                     else
//                     {
//                         importeAnterior += (denominacion.Cantidad * denominacion.DenominacionDeMoneda.Valor / 24);
//                     }
//                 }
//             }
//             //var desgloseEntregado = _db.Set<DenominacionEntregaDiaria>().Include(d => d.DenominacionDeMoneda.Moneda).Where(d => d.CierreDeCaja.DiaContableId == id);
//             var cierre = new CierreViewModel()
//             {
//                 Fecha = dia.Fecha,
//                 Desgloce = cierreCaja != null ? cierreCaja.Desglose.ToList() : null,
//                 EfectivoAnterior = importeAnterior,
//                 Efectivo = efectivo,
//             };

//             List<Type> typeToRegisters = new List<Type>();
//             foreach (var module in GlobalConfiguration.Modules)
//             {
//                 typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
//             }
//             var loaderTypes = typeToRegisters.Where(x => typeof(ICierreContableLoader).IsAssignableFrom(x));
//             foreach (var loaderType in loaderTypes)
//             {
//                 if (loaderType != null && loaderType != typeof(ICierreContableLoader))
//                 {
//                     var loader = (ICierreContableLoader)Activator.CreateInstance(loaderType, _db);
//                     cierre.Detalles.AddRange(loader.CargarResumen(id));
//                 }
//             }

//             return cierre;
//         }

//         public IEnumerable<ICierreContableElement> CargarResumen(int id)
//         {
//             var detalles = new List<ICierreContableElement>();
//             var dia = _db.Set<DiaContable>().Find(id);
//             var extracciones = _cuentasServices.GetMovimientosDeCuenta("Caja")
//                 .Any(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Extracción") || m.Asiento.Detalle.StartsWith("Pago") || m.Asiento.Detalle.StartsWith("Compra")))?
//                 _cuentasServices.GetMovimientosDeCuenta("Caja")
//                 .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Extracción") || m.Asiento.Detalle.StartsWith("Pago") || m.Asiento.Detalle.StartsWith("Compra"))).Sum(m => m.Importe):0;

//             var depositos = _cuentasServices.GetMovimientosDeCuenta("Caja")
//                 .Any(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Debito && m.Asiento.Detalle.StartsWith("Deposito"))?
//                 _cuentasServices.GetMovimientosDeCuenta("Caja")
//                 .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Debito && m.Asiento.Detalle.StartsWith("Deposito")).Sum(m => m.Importe):0;

//             detalles.Add(new CierreCajaElement() { Descripcion = "Depositos", Importe = depositos, Factor = 1 });
//             detalles.Add(new CierreCajaElement() { Descripcion = "Extracciones", Importe = extracciones, Factor = -1 });

//             return detalles;
//         }

//         public IEnumerable<ICierreContableElement> CargarResumen(DateTime fechaInicio, DateTime fechaFin)
//         {
//             throw new NotImplementedException();
//         }

//         public IEnumerable<ICierreContableElement> CargarResumen()
//         {
//             throw new NotImplementedException();
//         }

//         public CierreValidation SePuedeCerrar(int id)
//         {
//             var validation = new CierreValidation() { Valido = true };
//             var errores = new List<string>();
//             List<Type> typeToRegisters = new List<Type>();
//             foreach (var module in GlobalConfiguration.Modules)
//             {
//                 typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
//             }
//             var loaderTypes = typeToRegisters.Where(x => typeof(ICierreContableLoader).IsAssignableFrom(x));
//             foreach (var loaderType in loaderTypes.Where(x => !typeof(ResumenCierre).IsAssignableFrom(x)))
//             {
//                 if (loaderType != null && loaderType != typeof(ICierreContableLoader))
//                 {
//                     var loader = (ICierreContableLoader)Activator.CreateInstance(loaderType, _db);
//                     var val = loader.SePuedeCerrar(id);
//                     if (!val.Valido)
//                     {
//                         validation.Valido = false;
//                         errores.AddRange(val.Errores);
//                     }
//                 }
//             }
//             validation.Errores = errores;
//             return validation;
//         }


//     }
// }