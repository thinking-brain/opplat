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

            // for (int i = 1; i < 13; i++)
            // {
            //     decimal real = _obtenetPlan.ObtenerTotalIngresos(year, i);                
            //     decimal plan = _obtenetPlan.ObtenerTotalPlanIngresos(year, i);
            //     resultado.Reales.Add(real);
            //     resultado.Planes.Add(plan);
            // }
            resultado.Plan = 256987.55M;
            resultado.Real = 236589.90M;
            resultado.Planes = new List<decimal>() { 23455.90M, 2323.55M, 2323.20M, 2323.55M, 2323.90M, 2323.55M, 43545.30M, 4646.25M, 46464.55M, 3424M, 67879M, 5656M };
            resultado.Reales = new List<decimal>() { 6789.90M, 23232.25M, 454646.50M, 23232.00M, 23232.00M, 23232.00M, 2323.00M, 2323.00M, 2323.00M, 2323.00M, 2323.00M, 2323.00M };
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
        // [HttpGet("egresosTotal/{años}")]
        // public TotalIngresosViewModel EgresosTotal([FromRoute] string años)
        // {
        //     var resultado = new TotalIngresosViewModel();

        //     int year = Convert.ToInt32(años);

        //     for (int i = 1; i < 13; i++)
        //     {
        //         var real = _obtenetPlan.ObtenerTotalEgresos(year, i);
        //         var plan = _obtenetPlan.ObtenerTotalPlanEgresos(year, i);
        //         resultado.Reales.Add(real);
        //         resultado.Planes.Add(plan);
        //     }
        //     return resultado;
        // }
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

            // for (int i = 1; i < 13; i++)
            // {
            //     var real = _obtenetPlan.ObtenerTotalUtilidades(year, i);
            //     var plan = _obtenetPlan.ObtenerTotalPlanUtilidades(year, i);
            //     resultado.Reales.Add(real);
            //     resultado.Planes.Add(plan);
            // }
            return resultado;
        }
    }
}