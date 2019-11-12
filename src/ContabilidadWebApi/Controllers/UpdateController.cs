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
    public class UpdateController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public UpdateController(ContabilidadDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> VariablesEstadoFinanciero()
        {
            var data = DatosVariablesEstadoFinanciero.Datos().ToList();

            foreach (var item in data)
            {
                var cuentaEnParticipacion = new VariablesEstadoFinanciero { Nombre = item.Dato };
                _context.Add(cuentaEnParticipacion);

                string cc = item.Valor.ToString();
                string[] cuentas = cc.Split(',');


                for (int i = 0; i < cuentas.Length; i++)
                {
                    string cnt = cuentas[i].ToString();
                    if (_context.Set<Cuenta>().Any(s => s.NumeroParcial.Equals(cnt)))
                    {
                        var cuenta = _context.Set<Cuenta>().SingleOrDefault(s => s.NumeroParcial.Equals(cnt));
                        _context.Set<VariablesEstadoFinancieroCuentas>().Add(new VariablesEstadoFinancieroCuentas { CuentaId = cuenta.Id, VariablesEstadoFinanciero = cuentaEnParticipacion });
                    }
                }
            }
            _context.SaveChanges();

            return Ok();
        }


    }
}