using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class ObtenerPlanGI
    {
        IConfiguration _config { get; set; }

        public ObtenerPlanGI(IConfiguration config)
        {
            _config = config;
        }

        public List<PlanGIViewModel> ObtenerIngresos(int year, int meses)
        {
            // List<PlanGIResultVM> planGI = GetPlanGI.Get(_config);
            // var planesGI = planGI != null? planGI.Where(s => s.Año == Convert.ToString(year)):null;
            var plan = new List<PlanGIViewModel>();
            var modelo = DatosPlanGI.Datos();
           var planes = GetPlanIngresosPeriodo.Get(year, _config);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Ingresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                decimal ImporteAcumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
                var planEnMes = planes.SingleOrDefault(s=>s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s=>s.Concepto.Equals(concepto))[meses]: 0M;
                var planAcum = planes.SingleOrDefault(s=>s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s=>s.Concepto.Equals(concepto)).Acumulado(meses): 0M;
                
                var PlanMes = planEnMes;
                var PlanAcumulado = planAcum;
                var RealMes = Importe;
                var RealAcumulado = 0M;
                var TotalIngresos = 0M;

                var cost = new PlanGIViewModel
                {
                    //Grupo
                    Grupo = item.Dato.ToString(),
                    //Plan Mensual
                    PlanMes = PlanMes,
                    //Real del Mes
                    RealMes = RealMes,

                    // % de Cumplimiento
                    PorcCumplimiento = PlanMes > 0 ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos
                    PorcRelacionIngresos = null,
                    //% de los gastos en función del total
                    PorcGastosFuncionTotal = null,
                    //Plan Acumulado
                    PlanAcumulado = PlanAcumulado,
                    //Real Acumulado
                    RealAcumulado = RealAcumulado,
                    //% Cumplimiento
                    PorcCumpAcumulado = PlanAcumulado > 0 ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos 
                    PorcIngresosFuncionTotal = null,
                    //% de los gastos en función del total 
                    PorcGastosFuncionTotalAcumulado = null,
                    ////Total en el Grupo
                    //

                };
                plan.Add(cost);
            }

             return plan;

        }
    }

}
