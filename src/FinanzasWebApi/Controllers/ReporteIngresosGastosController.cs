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
    public class ReporteIngresosGastosController : ControllerBase
    {
        ObtenerPlanGI _obtenetPlan { get; set; }

        public ReporteIngresosGastosController(ObtenerPlanGI obtenerPlan)
        {
            _obtenetPlan = obtenerPlan;
        }

        /// <summary>
        /// Devuelve los Ingresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("ingresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Ingresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();

            int year = Convert.ToInt32(años);
            plan = _obtenetPlan.ObtenerIngresos(year, meses);

            var planes = new List<PlanGIViewModel>();

            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }
        /// <summary>
        /// Devuelve una lista con el real y el plan por meses en el año de los Ingresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <returns></returns>
        [HttpGet("ingresosTotal/{años}")]
        public ActionResult IngresosTotal([FromRoute] string años)
        {
            var resultado = new TotalIngresosViewModel();

            int year = Convert.ToInt32(años);

            for (int i = 1; i < 13; i++)
            {
                decimal real = _obtenetPlan.ObtenerTotalIngresos(year, i);
                decimal plan = _obtenetPlan.ObtenerTotalPlanIngresos(year, i);
                resultado.Reales.Add(real);
                resultado.Planes.Add(plan);
            }
            return Ok(resultado);
        }

        /// <summary>
        /// Devuelve los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("egresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Egresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();

            int year = Convert.ToInt32(años);
            plan = _obtenetPlan.ObtenerEgresos(year, meses);

            var planes = new List<PlanGIViewModel>();

            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }

        /// <summary>
        /// Devuelve una lista con el real y el plan por meses en el año de los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <returns></returns>
        [HttpGet("egresosTotal/{años}")]
        public TotalIngresosViewModel EgresosTotal([FromRoute] string años)
        {
            var resultado = new TotalIngresosViewModel();

            int year = Convert.ToInt32(años);

            for (int i = 1; i < 13; i++)
            {
                var real = Math.Round(_obtenetPlan.ObtenerTotalEgresos(year, i), 2);
                var plan = Math.Round(_obtenetPlan.ObtenerTotalPlanEgresos(year, i), 2);
                resultado.Reales.Add(real);
                resultado.Planes.Add(plan);
            }
            return resultado;
        }
        /// <summary>
        /// Devuelve los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("utilidad/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Utilidad([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();

            int year = Convert.ToInt32(años);
            plan = _obtenetPlan.ObtenerUtilidad(year, meses);

            var planes = new List<PlanGIViewModel>();

            foreach (var item in plan)
            {
                planes.Add(item);
            }
            return planes;
        }

        /// <summary>
        /// Devuelve una lista con el real y el plan por meses en el año de los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <returns></returns>
        [HttpGet("utilidadesTotal/{años}")]
        public TotalIngresosViewModel UtilidadesTotal([FromRoute] string años)
        {
            var resultado = new TotalIngresosViewModel();

            int year = Convert.ToInt32(años);

            for (int i = 1; i < 13; i++)
            {
                var real = _obtenetPlan.ObtenerTotalUtilidades(year, i);
                var plan = _obtenetPlan.ObtenerTotalPlanUtilidades(year, i);
                resultado.Reales.Add(real);
                resultado.Planes.Add(plan);
            }
            return resultado;
        }
    }
}