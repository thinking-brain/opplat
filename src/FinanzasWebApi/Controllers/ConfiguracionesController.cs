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
    public class ConfiguracionesController : ControllerBase
    {
        FinanzasDbContext _context { get; set; }

        public ConfiguracionesController(FinanzasDbContext context)
        {
            _context = context;
        }

        private List<ConfigDto> Configs()
        {
            var configs = _context.Set<ConfiguracionPorciento>().Select(c => new ConfigDto
            {
                Nombre = c.Titulo,
                Valor = c.Porciento.ToString()
            }).ToList();
            configs.AddRange(_context.Set<ConfiguracionFirmas>().Select(c => new ConfigDto
            {
                Nombre = c.Nombre,
                Valor = c.Valor
            }));
            return configs;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Configs());
        }

        [HttpPost]
        public IActionResult Post(ConfigDto config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            if (config.Nombre.Contains("Porciento"))
            {
                _context.Add(new ConfiguracionPorciento { Titulo = config.Nombre, Porciento = double.Parse(config.Valor) });
            }
            else
            {
                _context.Add(new ConfiguracionFirmas { Nombre = config.Nombre, Valor = config.Valor });
            }
            _context.SaveChanges();
            return Ok(config);
        }

        [HttpPut("/{key}")]
        public IActionResult Put(string key, ConfigDto config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            if (key.Contains("Porciento"))
            {
                var porciento = _context.Set<ConfiguracionPorciento>().SingleOrDefault(c => c.Titulo == key);
                if (porciento == null)
                {
                    return NotFound("No se encuentra la configuracion");
                }
                porciento.Porciento = double.Parse(config.Valor);
            }
            else
            {
                var porciento = _context.Set<ConfiguracionFirmas>().SingleOrDefault(c => c.Nombre == key);
                if (porciento == null)
                {
                    return NotFound("No se encuentra la configuracion");
                }
                porciento.Valor = config.Valor;
            }
            _context.SaveChanges();
            return Ok(config);
        }

        [HttpDelete("/{key}")]
        public IActionResult Delete(string key)
        {
            if (key.Contains("Porciento"))
            {
                var porciento = _context.Set<ConfiguracionPorciento>().SingleOrDefault(c => c.Titulo == key);
                if (porciento == null)
                {
                    return NotFound("No se encuentra la configuracion");
                }
                _context.Remove(porciento);
            }
            else
            {
                var porciento = _context.Set<ConfiguracionFirmas>().SingleOrDefault(c => c.Nombre == key);
                if (porciento == null)
                {
                    return NotFound("No se encuentra la configuracion");
                }
                _context.Remove(porciento);
            }
            _context.SaveChanges();
            return Ok();
        }
    }
}