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
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class UpdateCacheController : ControllerBase
    {
        FinanzasDbContext _context { get; set; }
        IConfiguration _config;

        public UpdateCacheController(FinanzasDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
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
            //buscando cuentas
            var subMCuentas = new List<CuentaDto>();
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                HttpClient client = new HttpClient(handler);

                var url = _config.GetValue<string>("ContabilidadApi") + "/Cuentas";
                var result = client.GetAsync(url).Result;
                if (result.IsSuccessStatusCode)
                {
                    subMCuentas = result.Content.ReadAsAsync<List<CuentaDto>>().Result;
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest("No se puede consultar las cuentas. " + ex.Message);
            }
            var errores = new List<string>();
            foreach (var cuenta in subMCuentas.Where(s => s.Numero.Length == 3))
            {
                var result = UpdateCuenta(cuenta.Numero, mes, year);
                if (!result)
                {
                    errores.Add($"No se pudo actualizar la cuenta: {cuenta.Numero}");
                }
            }
            if (errores.Count > 0)
            {
                return Ok(new { Mensaje = "Se actualizaron las cache de las cuentas con errores.", Errors = errores });
            }
            return Ok("Actualizacion correcta.");
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
            var result = UpdateCuenta(cuenta, mes, year);
            if (!result)
            {
                return BadRequest("Error actualizando la cache de la cuenta.");
            }
            return Ok("Actualizacion realizada correctamente.");
        }

        private bool UpdateCuenta(string cuenta, int mes, int year)
        {
            try
            {
                var saldo = GetMovimientoDeCuentaPeriodo.Get(year, mes, cuenta, _config);
                var acumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, mes, cuenta, _config);
                var data = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cuenta && c.Mes == mes && c.Year == year);
                if (data == null)
                {
                    _context.Add(new CacheCuentaPeriodo
                    {
                        Cuenta = cuenta,
                        FechaActualizado = DateTime.Now,
                        Mes = mes,
                        Year = year,
                        Saldo = saldo,
                        Acumulado = acumulado
                    });
                }
                else
                {
                    data.Saldo = saldo;
                    data.Acumulado = acumulado;
                    data.FechaActualizado = DateTime.Now;
                }
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}