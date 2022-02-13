using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using op_costos_api.Data;
using op_costos_api.Helper;
using op_costos_api.Models;
using op_costos_api.VersatModels;
using op_costos_api.VersatModels2;
using op_costos_api.ViewModels;

namespace op_costos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanIGController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly VersatDbContext2 _v2context;

        public PlanIGController(ApiDbContext context, VersatDbContext vcontext, VersatDbContext2 v2context)
        {
            _context = context;
            _vcontext = vcontext;
            _v2context = v2context;
        }
        /// <summary>
        /// Subir Plan de Gastos e Ingresos del Año
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        // GET api/values
        [HttpPost, Route("api/UploadPlanGI/")]
        public async Task<IActionResult> UploadPlanGI(IFormFile File)
        {
            try
            {
                using (StreamReader reader = new StreamReader(File.OpenReadStream()))
                {
                    var csvReader = new CsvReader(reader);
                    var planHelper = new PlanGICsvHelper(_context);
                    csvReader.Configuration.Delimiter = "|";
                    planHelper.readCsv(csvReader);
                }
                return Ok(); ;
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
        /// <summary>
        /// Devuelve los datos necesarios para crear un Plan IG
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/PlanIGDatos/")]
        public List<dynamic> PlanIGDatos()
        {
            var datos = new List<dynamic>();
            foreach (var item in DatosPlanGI.Datos())
            {
                datos.Add(new { Valor = item.Valor, Dato = item.Dato });
            }
            return datos;
        }

        /// <summary>
        /// /Crear Planes GI
        /// </summary>
        /// <param name="planes"></param>
        /// <returns></returns>
        [HttpPost("api/PlanIGCreate/")]
        public IActionResult PlanIGCreate([FromRoute] List<PlanGI> planes)
        {
            foreach (var item in planes)
            {
                _context.PlanGI.Add(item);
                _context.SaveChanges();
            }
            return Ok();
        }


        /// <summary>
        /// Devuelve los Ingresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("api/Ingresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Ingresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context(_vcontext, _v2context, _context).ObtenerIngresos(year, meses);
            return plan;
        }

        /// <summary>
        /// Devuelve los Egresos para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("api/Egresos/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Egresos([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context(_vcontext, _v2context, _context).ObtenerEgresos(year, meses);
            return plan;
        }

        /// <summary>
        /// Devuelve las Utilidades para el plan de Gastos en Ingresos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("api/Utilidades/{años}/{meses}")]
        public IEnumerable<PlanGIViewModel> Utilidades([FromRoute] string años, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPlanGI_Context(_vcontext, _v2context, _context).ObtenerUtilidades(year, meses);
            return plan;
        }
    }
}