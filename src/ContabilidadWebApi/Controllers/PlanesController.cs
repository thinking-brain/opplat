using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Helper;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.Data;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class PlanesController : ControllerBase
    {
        public class Resutado
        {
            public bool error { get; set; }

            public string mensaje { get; set; }
        }

        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;


        public PlanesController(ApiDbContext context, VersatDbContext vcontext)
        {
            _context = context;
            _vcontext = vcontext;
        }
        /// <summary>
        /// Subir Plan de Gastos desglozado por SubElementos
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        // GET api/values
        [HttpPost, Route("UploadPlan/")]
        public async Task<IActionResult> UploadPlan(IFormFile File)
        {

            try
            {
                using (StreamReader reader = new StreamReader(File.OpenReadStream()))
                {
                    var csvReader = new CsvReader(reader);
                    var planHelper = new PlanCsvHelper(_vcontext, _context);
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
        /// Devuelve un Listado de Planes
        /// </summary>
        /// <returns></returns>
        // GET: api/Planes
        [HttpGet]
        public IEnumerable<Plan> GetPlan()
        {
            return _context.Plan;
        }

        /// <summary>
        /// Chequea que Exista un Plan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool PlanExists(int id)
        {
            return _context.Plan.Any(e => e.Id == id);
        }
    }
}