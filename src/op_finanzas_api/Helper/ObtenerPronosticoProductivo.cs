using Microsoft.EntityFrameworkCore;
using op_finanzas_api.Data;
using op_finanzas_api.Helpers;
using op_finanzas_api.Models;
using op_finanzas_api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace op_finanzas_api.Helper
{
    public class ObtenerPronosticoProductivo
    {

        // public List<PronosticoProductivoVM> Obtener(int year, int meses)
        // {
        //    //SUBMAYOR DE CUENTAS
        //     HttpClient client = new HttpClient();
        //     List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
        //     var result = client.GetAsync("http://localhost:5050/api/SubmayorDeCuentas").Result;
        //     if (result.IsSuccessStatusCode)
        //     {
        //         subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
        //     }
        //     //Plan Pronostico Productivo
        //     HttpClient clientPP = new HttpClient();
        //     List<PlanPronosticoProductivoVM> planPP = new List<PlanPronosticoProductivoVM>();
        //     var resultPP = clientPP.GetAsync( "http://localhost:5050/api/PlanIG/api/PlanesIG").Result;
        //     if (resultPP.IsSuccessStatusCode)
        //     {
        //         planPP = resultPP.Content.ReadAsAsync<List<PlanPronosticoProductivoVM>>().Result;
        //     }
        //     var plan = new List<PronosticoProductivoVM>();

        //     //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
        //     var data = subMCuentas;

        //     //Buscando el Primer Periodo del Año consultado
        //     var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();

        //     //Seleccionando solo los movimientos dentro del periodo del mes consultado
        //     var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();

        //     //Seleccionando todos los movimientos que esten entre el comienzo del año y sean menores e igual al mes consultado
        //     var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

        //     //Tabla que contiene los planes de Pronosticos Productivos
        //     var planesPP = planPP.Where(s => s.Año == Convert.ToString(year));

           

        //     foreach (var item in dataInMes)
        //     {
        //         var Acumulado = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals(item.ClaveCuenta)).Sum(s => s.Importe) * -1;
        //         var cuentaNat = _vcontext.ConCuentanat.SingleOrDefault(s => s.Idcuenta == item.IdCuenta);
        //         var clav = item.ClaveCuenta.TrimEnd().Split(' ');
        //         if (cuentaNat.Naturaleza == -1 && clav.Length > 1)
        //         {
        //             var cuenta = _vcontext.ConCuenta.SingleOrDefault(s => s.Idcuenta == item.IdCuenta);
        //             string moneda = _vcontext.ConCuentamoneda.SingleOrDefault(s => s.Idcuenta == cuenta.Idcuenta).Idmoneda == 2 ? "CUC" : "CUP";
        //             var conceptosCta = _context.Set<ConceptoCuentas>().SingleOrDefault(s => s.CuentaId == cuenta.Idcuenta);
        //             if (conceptosCta != null)
        //             {
        //                 var conceptosPlan = _context.Set<ConceptoPlan>().SingleOrDefault(s => s.Id == conceptosCta.ConceptoPlanId);

        //                 if (conceptosPlan != null)
        //                 {
        //                     var planes = _context.Set<PlanPronosticoProductivo>().Where(s => s.ConceptoPlanId == conceptosPlan.Id);

        //                     if (!plan.Any(s=>s.Obra.Trim().ToUpper().Equals(conceptosPlan.Concepto.Trim().ToUpper())) && item.Importe != 0)
        //                     {
        //                         if (moneda.Equals("CUC"))
        //                         {
        //                             plan.Add(new PronosticoProductivoVM { Obra = conceptosPlan.Concepto, CUC = (item.Importe * -1), PlanMes = planes.Count() >0 ? planes.First()[meses] : 0M, AcumuladoCUC = Acumulado });

        //                         }
        //                         if (moneda.Equals("CUP"))
        //                         {
        //                             plan.Add(new PronosticoProductivoVM { Obra = conceptosPlan.Concepto, CUP = (item.Importe * -1), PlanMes = planes.Count() >0 ? planes.First()[meses] : 0M, AcumuladoCUP = Acumulado });
        //                         }
        //                     }
        //                     else
        //                     {
        //                         var p = plan.SingleOrDefault(s => s.Obra.Trim().ToUpper().Equals(conceptosPlan.Concepto.Trim().ToUpper()));
        //                         if (moneda.Equals("CUC"))
        //                         {
        //                             p.CUC = p.CUC + (item.Importe * -1);
        //                             p.AcumuladoCUC = p.AcumuladoCUC + Acumulado;
        //                         }
        //                         if (moneda.Equals("CUP"))
        //                         {
        //                             p.CUP = p.CUP + (item.Importe * -1);
        //                             p.AcumuladoCUP = p.AcumuladoCUP + Acumulado;
        //                         }
        //                     }

        //                     foreach (var pp in planesPP.Include(s=>s.ConceptoPlan))
        //                     {

        //                         if (!plan.Any(s => s.Obra.Trim().ToUpper().Equals(pp.ConceptoPlan.Concepto.Trim().ToUpper())))
        //                         {

        //                             if (moneda.Equals("CUC"))
        //                             {
        //                                 plan.Add(new PronosticoProductivoVM { Obra = pp.ConceptoPlan.Concepto, CUC = 0M, PlanMes = planes != null ? pp[meses] : 0M });
        //                             }
        //                             if (moneda.Equals("CUP"))
        //                             {
        //                                 plan.Add(new PronosticoProductivoVM { Obra = pp.ConceptoPlan.Concepto, CUP = 0M, PlanMes = planes != null ? pp[meses] : 0M });
        //                             }
        //                         }
        //                         //else
        //                         //{
        //                         //    var p = plan.SingleOrDefault(s => s.Obra.Trim().ToUpper().Equals(pp.ConceptoPlan.Concepto.Trim().ToUpper()));
        //                         //    if (moneda.Equals("CUC"))
        //                         //    {
        //                         //        p.CUC = p.CUC + (item.Importe * -1);
        //                         //    }
        //                         //    if (moneda.Equals("CUP"))
        //                         //    {
        //                         //        p.CUP = p.CUP + (item.Importe * -1);
        //                         //    }
        //                         //}

        //                     }
        //                 }
        //             }
                   
        //             //var planes = _context.Set<PlanPronosticoProductivo>().Where(s => s.CuentaMN == Convert.ToString(cuentaNat.Idcuenta).Trim() || s.CuentaCUC == Convert.ToString(cuentaNat.Idcuenta).Trim());


        //         }
        //     }
        //     return plan;
        // }

    }

}
