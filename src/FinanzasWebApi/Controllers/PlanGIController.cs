using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.Helper;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class PlanGIController : ControllerBase
    {
       
        /// <summary>
        /// Devuelve los Ingresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("Ingresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Ingresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
          
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context().ObtenerIngresos(year, meses);

            var planes = new List<PlanGIViewModel>();
            planes.Add(new PlanGIViewModel {
                Grupo = "Ingresos",
                PlanMes = plan.Sum(s => s.PlanMes),
                RealMes = plan.Sum(s => s.RealMes),
                PorcCumplimiento = plan.Sum(s => s.PorcCumplimiento),
                PorcRelacionIngresos = plan.Sum(s => s.PorcRelacionIngresos),
                PorcGastosFuncionTotal = plan.Sum(s => s.PorcGastosFuncionTotal),
                PlanAcumulado = plan.Sum(s => s.PlanAcumulado),
                RealAcumulado = plan.Sum(s => s.RealAcumulado),
                PorcCumpAcumulado = plan.Sum(s => s.PorcCumpAcumulado),
                PorcIngresosFuncionTotal = plan.Sum(s => s.PorcIngresosFuncionTotal),
                PorcGastosFuncionTotalAcumulado = plan.Sum(s => s.PorcGastosFuncionTotalAcumulado),

            });
            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }

        /// <summary>
        /// Devuelve los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("Egresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Egresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context().ObtenerEgresos(year, meses);
            var planes = new List<PlanGIViewModel>();
            planes.Add(new PlanGIViewModel
            {
                Grupo = "Egresos",
                PlanMes = plan.Sum(s => s.PlanMes),
                RealMes = plan.Sum(s => s.RealMes),
                PorcCumplimiento = plan.Sum(s => s.PorcCumplimiento),
                PorcRelacionIngresos = plan.Sum(s => s.PorcRelacionIngresos),
                PorcGastosFuncionTotal = plan.Sum(s => s.PorcGastosFuncionTotal),
                PlanAcumulado = plan.Sum(s => s.PlanAcumulado),
                RealAcumulado = plan.Sum(s => s.RealAcumulado),
                PorcCumpAcumulado = plan.Sum(s => s.PorcCumpAcumulado),
                PorcIngresosFuncionTotal = plan.Sum(s => s.PorcIngresosFuncionTotal),
                PorcGastosFuncionTotalAcumulado = plan.Sum(s => s.PorcGastosFuncionTotalAcumulado),

            });
            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }

        /// <summary>
        /// Devuelve las Utilidades para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("Utilidades/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Utilidades([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context().ObtenerUtilidades(year, meses);
            var planes = new List<PlanGIViewModel>();
            decimal TotalIngresos = GetTotalIngresosEnMes.GetReal(year, meses);
            decimal TotalPlanIngresos = GetTotalIngresosEnMes.GetPlan(year, meses);
            decimal TotalIngresosAcumulado = GetTotalIngresosEnMes.GetRealAcumulado(year, meses);
            decimal TotalPlanIngresosAcumulado = GetTotalIngresosEnMes.GetPlanAcumulado(year, meses);

            decimal TotalEgresos = GetTotalEgresosEnMes.GetReal(year, meses);
            decimal TotalPlanEgresos = GetTotalEgresosEnMes.GetPlan(year, meses);
            decimal TotalEgresosAcumulado = GetTotalEgresosEnMes.GetRealAcumulado(year, meses);
            decimal TotalPlanEgresosAcumulado = GetTotalEgresosEnMes.GetPlanAcumulado(year, meses);

            decimal PlanMes = TotalPlanIngresos - TotalPlanEgresos;
            decimal RealMes =  TotalIngresos - TotalEgresos; 
            decimal PlanAcumulado = TotalPlanIngresosAcumulado - TotalPlanEgresosAcumulado;
            decimal RealAcumulado = TotalIngresosAcumulado - TotalEgresosAcumulado;

           
            planes.Add(new PlanGIViewModel
            {
                Grupo = "Utilidad",
                PlanMes = PlanMes,
                RealMes = RealMes,
                PorcCumplimiento = Math.Round(RealMes / PlanMes, 2, MidpointRounding.AwayFromZero),
                PorcRelacionIngresos = plan.Sum(s => s.PorcRelacionIngresos),
                PorcGastosFuncionTotal = plan.Sum(s => s.PorcGastosFuncionTotal),
                PlanAcumulado = Math.Round(PlanAcumulado, 2, MidpointRounding.AwayFromZero),
                RealAcumulado = Math.Round(RealAcumulado, 2, MidpointRounding.AwayFromZero),
                PorcCumpAcumulado = plan.Sum(s => s.PorcCumpAcumulado),
                PorcIngresosFuncionTotal = plan.Sum(s => s.PorcIngresosFuncionTotal),
                PorcGastosFuncionTotalAcumulado = plan.Sum(s => s.PorcGastosFuncionTotalAcumulado),

            });
            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }
    }
}