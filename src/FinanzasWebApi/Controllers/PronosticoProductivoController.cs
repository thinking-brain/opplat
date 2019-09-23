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
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PronosticoProductivoController : ControllerBase
    {
        private readonly ApiDbContext _context;
     

        public PronosticoProductivoController(ApiDbContext context)
        {
            _context = context;
        }
        

        [HttpGet("api/Obras/")]
        public IEnumerable<PlanPronosticoProductivoVM> Obras()
        {
            var plan = new List<PlanPronosticoProductivoVM>();

            var obras = _context.Set<ConceptoPlanVM>().ToList();
            foreach (var item in obras)
            {
                plan.Add(new PlanPronosticoProductivoVM
                {
                    ConceptoPlan = item,
                    ConceptoPlanVMId = item.Id,
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
            // plan = new ObtenerPronosticoProductivo().Obtener(year, meses);
            return plan;
        }

       
    }
}