using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using op_contabilidad_api.Helper;
using op_contabilidad_api.Models;
using op_contabilidad_api.VersatModels;
using op_contabilidad_api.VersatModels2;
using op_contabilidad_api.ViewModels;

namespace op_contabilidad_api.Controllers
{
    [Route("api/[controller]")]
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
