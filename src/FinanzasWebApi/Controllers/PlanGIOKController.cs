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
    public class PlanGIOKController : ControllerBase
    {
        ObtenerPlanGI _obtenetPlan { get; set; }

        public PlanGIOKController(ObtenerPlanGI obtenerPlan)
        {
            _obtenetPlan = obtenerPlan;
        }

        /// <summary>
        /// Devuelve los Ingresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Ingresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();

            int year = Convert.ToInt32(años);
            plan = _obtenetPlan.ObtenerIngresos(year, meses);

            var planes = new List<PlanGIViewModel>();
            // planes.Add(new PlanGIViewModel
            // {
            //     Grupo = "Ingresos",
            //     PlanMes = plan.Sum(s => s.PlanMes),
            //     RealMes = plan.Sum(s => s.RealMes),
            //     PorcCumplimiento = plan.Sum(s => s.PorcCumplimiento),
            //     PorcRelacionIngresos = null,
            //     PorcGastosFuncionTotal = null,
            //     PlanAcumulado = plan.Sum(s => s.PlanAcumulado),
            //     RealAcumulado = plan.Sum(s => s.RealAcumulado),
            //     PorcCumpAcumulado = plan.Sum(s => s.PorcCumpAcumulado),
            //     PorcIngresosFuncionTotal = null,
            //     PorcGastosFuncionTotalAcumulado = null,

            // });
            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }


    }
}