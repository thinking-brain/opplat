using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Helper;
using ContabilidadWebApi.Models;
using Newtonsoft.Json;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class PlanGIController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public PlanGIController(ContabilidadDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve los Planes de IG
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PlanesGI()
        {
            var planes = _context.Set<PlanGI>().Include(p => p.Detalles)
            .ThenInclude(p => p.Concepto).Select(
                s => new
                {
                    id = s.Id,
                    nombre = s.Titulo,
                    year = s.Year
                }).ToList();
            return Ok(planes);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var detalles = _context.Set<DetallePlanGI>().Include(d => d.Concepto )
            .Where(d => d.PlanId == id).OrderBy(d => d.Id).ToList();
               
            return Ok(detalles);
        }


        /// <summary>
        /// Subir Plan de Gastos e Ingresos del Año
        /// </summary>
        /// <param name="File"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        // GET api/values
        [HttpPost, Route("UploadPlanGI/")]
        public async Task<IActionResult> UploadPlanGI(IFormFile File, [FromForm]string year)
        {
            try
            {
                using (StreamReader reader = new StreamReader(File.OpenReadStream()))
                {
                    var csvReader = new CsvReader(reader);
                    var planHelper = new PlanGICsvHelper(_context);
                    csvReader.Configuration.Delimiter = "|";
                    planHelper.readCsv(csvReader, year);
                }
                return Ok();
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
        [HttpGet("PlanGIDatos/")]
        public List<dynamic> PlanGIDatos()
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
        [HttpPost("PlanGICreate/")]
        public IActionResult PlanGICreate([FromRoute] List<PlanGI> planes)
        {
            foreach (var item in planes)
            {
                _context.Set<PlanGI>().Add(item);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}