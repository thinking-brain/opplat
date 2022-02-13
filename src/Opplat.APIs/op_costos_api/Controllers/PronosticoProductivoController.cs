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
    public class PronosticoProductivoController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly VersatDbContext2 _v2context;

        public PronosticoProductivoController(ApiDbContext context, VersatDbContext vcontext, VersatDbContext2 v2context)
        {
            _context = context;
            _vcontext = vcontext;
            _v2context = v2context;
        }
        ///// <summary>
        ///// Subir Plan de Pronosticos Productivos
        ///// </summary>
        ///// <param name="File"></param>
        ///// <returns></returns>
        //// GET api/values
        //[HttpPost, Route("api/UploadPlanPP/")]
        //public async Task<IActionResult> UploadPlanPP(IFormFile File)
        //{
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(File.OpenReadStream()))
        //        {
        //            var csvReader = new CsvReader(reader);
        //            var planHelper = new PlanPPCsvHelper(_context, _vcontext);
        //            csvReader.Configuration.Delimiter = "|";
        //            planHelper.readCsv(csvReader);
        //        }
        //        return Ok(); ;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok();
        //    }
        //}


        [HttpGet("api/Obras/")]
        public IEnumerable<PlanPronosticoProductivo> Obras()
        {
            var plan = new List<PlanPronosticoProductivo>();

            var obras = _context.Set<ConceptoPlan>().ToList();
            foreach (var item in obras)
            {
                plan.Add(new PlanPronosticoProductivo
                {
                    ConceptoPlan = item,
                    ConceptoPlanId = item.Id,
                    Enero = 0M,
                    Febrero = 0M,
                    Marzo = 0M,
                    Abril = 0M,
                    Mayo = 0M,
                    Junio = 0M,
                    Julio = 0M,
                    Agosto = 0M,
                    Septiembre = 0M,
                    Octubre = 0M,
                    Noviembre = 0M,
                    Diciembre = 0M,
                    Fecha = DateTime.Now,
                    Año = ""
                });
            }
            return plan;
        }

        /// <summary>
        /// Crear Plan
        /// </summary>
        /// <param name="planes"></param>
        /// <returns></returns>
        // POST: api/PlanesPPPost
        [HttpPost("api/PlanesPPPost/")]
        public async Task<IActionResult> PlanesPPPost([FromBody] List<PlanPronosticoProductivo> planes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var plan in planes)
            {
                _context.PlanPronosticoProductivo.Add(plan);

            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Devuelve Pronosticos Productivos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("api/PP/{años}/{meses}")]
        public IEnumerable<PronosticoProductivoVM> PP([FromRoute] string años, int meses)
        {
            var plan = new List<PronosticoProductivoVM>();
            int year = Convert.ToInt32(años);
            plan = new ObtenerPronosticoProductivo(_vcontext, _v2context, _context).Obtener(year, meses);
            return plan;
        }

       
    }
}