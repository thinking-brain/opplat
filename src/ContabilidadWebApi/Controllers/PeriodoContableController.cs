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
    public class PeriodoContableController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public PeriodoContableController(ContabilidadDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve el periodo contable activo
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var periodo = _context.Set<PeriodoContable>().SingleOrDefault(p => p.Activo);
                return Ok(periodo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve un Periodo Contable por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPeriodo([FromRoute] int id)
        {
            try
            {
                var periodo = _context.Set<PeriodoContable>().SingleOrDefault(p => p.Id == id);
                if (periodo == null)
                {
                    return NotFound("El periodo contable no existe.");
                }
                return Ok(periodo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega un nuevo periodo contable
        /// </summary>
        /// <param name="periodo">Datos del nuevo periodo</param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Post([FromBody] PeriodoContable periodo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (_context.Set<PeriodoContable>().Any(p => p.FechaFin > periodo.FechaInicio || p.FechaInicio < periodo.FechaFin))
                {
                    return BadRequest("El periodo que desea crear esta comprendido dentro de otro periodo.");
                }
                foreach (var per in _context.Set<PeriodoContable>().Where(p => p.Activo))
                {
                    per.Activo = false;
                }
                periodo.Activo = true;
                _context.Add(periodo);
                _context.SaveChanges();
                return Ok(periodo);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifica una cuenta
        /// </summary>
        /// <param name="id">Id del periodo</param>
        /// <param name="periodo">Datos del periodo</param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<IActionResult> Put(int id, [FromBody] PeriodoContable periodo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Set<PeriodoContable>().Any(c => c.Id == id))
            {
                return NotFound($"No se encuentra el periodo con id {id}");
            }
            try
            {
                var periodoContable = _context.Set<PeriodoContable>().Find(id);
                periodoContable.FechaInicio = periodo.FechaInicio;
                periodoContable.FechaFin = periodo.FechaFin;
                periodoContable.Activo = periodo.Activo;
                _context.Update(periodoContable);
                _context.SaveChanges();
                return Ok(periodo);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Marca como inactivo un periodo contable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var periodo = await _context.Set<PeriodoContable>().FindAsync(id);
            if (periodo == null)
            {
                return NotFound($"No se encuentra el periodo contable con id {id}");
            }
            try
            {
                if (_context.Set<DiaContable>().Any(d => d.PeriodoContableId == id))
                {
                    periodo.Activo = false;
                    _context.Update(periodo);
                    await _context.SaveChangesAsync();
                    return Ok("El periodo tiene operaciones, no se puede eliminar pero se desactivo si estaba activo.");
                }
                else
                {
                    _context.Remove(periodo);
                    await _context.SaveChangesAsync();
                    return Ok("Periodo eliminado correctamente.");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}