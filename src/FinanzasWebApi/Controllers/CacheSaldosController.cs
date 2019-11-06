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
using FinanzasWebApi.Models;
using FinanzasWebApi.Dtos;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class CacheSaldosController : ControllerBase
    {
        FinanzasDbContext _context { get; set; }

        public CacheSaldosController(FinanzasDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve los saldos y acumulados de todas las cuentas en cache en un periodo
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("{mes}/{year}")]
        public IActionResult Get(int mes, int year)
        {
            var data = _context.Set<CacheCuentaPeriodo>().Where(c => c.Mes == mes && c.Year == year).ToList();
            if (data == null)
            {
                return NotFound("Las operaciones en el mes solicitado no estan en la cache.");
            }
            return Ok(data);
        }

        /// <summary>
        /// Devuelve el saldo y acumulado de una cuenta en un periodo si esta en cache
        /// </summary>
        /// <param name="cuenta"></param>
        /// <param name="mes"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("{cuenta}/{mes}/{year}")]
        public IActionResult Get(string cuenta, int mes, int year)
        {
            var data = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cuenta && c.Mes == mes && c.Year == year);
            if (data == null)
            {
                return NotFound("Las operaciones de la cuenta en el mes solicitado no estan en la cache.");
            }
            return Ok(data);
        }

    }
}