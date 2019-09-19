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

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class SubmayorDeCuentasController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public SubmayorDeCuentasController(ApiDbContext context)
        {
            _context = context;

        }

         /// <summary>
        /// Devuelve el SubMayor de Cuentas
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public IEnumerable<SubMayorCuenta> GetSubMayorCuenta()
        {
            return _context.SubMayorCuenta.ToList();
        }
    }
}
