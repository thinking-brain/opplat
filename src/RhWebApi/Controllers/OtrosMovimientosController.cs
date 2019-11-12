using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class OtrosMovimientosController : Controller {
        private readonly RhWebApiDbContext context;
        public OtrosMovimientosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/otroMovimiento
        [HttpGet]
        public IEnumerable<OtroMovimientoDto> GetAll () {
            var otroMovimiento = context.OtroMovimiento.Select (t => new OtroMovimientoDto {
                Id = t.Id,
                    TrabajadorId = t.TrabajadorId,
                    Trabajador = t.Trabajador.Nombre,
                    Desde = t.Desde,
                    Hasta = t.Hasta,
                    Nombre = t.Nombre,
            });
            return otroMovimiento;
        }

        // GET: api/otroMovimiento/Id
        [HttpGet ("{id}", Name = "GetOtrosMov")]
        public IEnumerable<OtroMovimientoDto> GetbyId (int id) {

            var otroMovimiento = context.OtroMovimiento
                .Where (s => s.Id == id)
                .Select (t => new OtroMovimientoDto {
                    Id = t.Id,
                        TrabajadorId = t.TrabajadorId,
                        Trabajador = t.Trabajador.Nombre,
                        Desde = t.Desde,
                        Hasta = t.Hasta,
                        Nombre = t.Nombre,
                });
            return otroMovimiento;
        }

        // POST api/otroMovimiento
        [HttpPost]
        public IActionResult POST ([FromBody] OtroMovimientoDto otroMovimientoDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == otroMovimientoDto.TrabajadorId);

                var otroMovimiento = new OtroMovimiento ();
                otroMovimiento.Id = otroMovimientoDto.Id;
                otroMovimiento.TrabajadorId = otroMovimientoDto.TrabajadorId;
                otroMovimiento.Fecha = otroMovimientoDto.CuandoSeHizo;
                otroMovimiento.Desde = otroMovimientoDto.Desde;
                otroMovimiento.Hasta = otroMovimientoDto.Hasta;
                otroMovimiento.Nombre = otroMovimientoDto.Nombre;

                context.OtroMovimiento.Add (otroMovimiento);

                trabajador.EstadoTrabajador = otroMovimientoDto.Nombre;
                context.SaveChanges ();

                return new CreatedAtRouteResult ("GetOtrosMov", new { id = otroMovimiento.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/otroMovimiento/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] OtroMovimiento otroMovimiento, int id) {
            if (otroMovimiento.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (otroMovimiento).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/otroMovimiento/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var otroMovimiento = context.OtroMovimiento.FirstOrDefault (s => s.Id == id);

            if (otroMovimiento.Id != id) {
                return NotFound ();
            }
            context.OtroMovimiento.Remove (otroMovimiento);
            context.SaveChanges ();
            return Ok (otroMovimiento);
        }
    }
}