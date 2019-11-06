using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Services;
using ContabilidadWebApi.Models;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class VariablesEFController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public VariablesEFController(ContabilidadDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve Datos
        /// </summary>
        /// <returns></returns>        
        [HttpGet("datosDeVariables/{dato}")]
        public string Dato(string dato)
        {
            var data = DatosVariablesEstadoFinanciero.Datos().SingleOrDefault(s => s.Dato.Trim().ToUpper().Equals(dato.Trim().ToUpper()));
            return (data.Valor.ToString());
        }



    }
}