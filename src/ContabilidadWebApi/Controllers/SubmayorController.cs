using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ContabilidadWebApi.Helper;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.VersatModels2;
using ContabilidadWebApi.ViewModels;
using ContabilidadWebApi.Data;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class SubmayorController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly VersatDbContext2 _v2context;

        public SubmayorController(ApiDbContext context, VersatDbContext vcontext, VersatDbContext2 v2context)
        {
            _context = context;
            _vcontext = vcontext;
            _v2context = v2context;
        }

        /// <summary>
        /// Devuelve el SubMayor
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public IEnumerable<SubMayor> GetSubMayor()
        {
            return _context.SubMayor.ToList();
        }


        /// <summary>
        /// Actualizar SubMayor desde la Base de Datos ED
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("Updates2/")]
        public ActionResult<IEnumerable<string>> Update2()
        {
            var report = new UpdateSubmayor2Helper(_v2context, _context);
            report.LlenarSubMayor2();
            return new string[] { "UPDATE" };
        }

        /// <summary>
        /// Actualizar SubMayor desde la Base de Datos VERSAT_2017
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("Updates/")]
        public ActionResult<IEnumerable<string>> Update()
        {
            var report = new UpdateSubmayorHelper(_vcontext, _context);
            report.LlenarSubMayor();
            return new string[] { "UPDATE" };
        }



    }
}
