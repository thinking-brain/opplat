using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class OtrosMovimientosController : Controller {
        private readonly RhWebApiDbContext context;
        public OtrosMovimientosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/otroMovimiento
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

        // GET: recursos_humanos/otroMovimiento/Id
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

        // POST recursos_humanos/otroMovimiento
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

        // PUT recursos_humanos/otroMovimiento/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] OtroMovimiento otroMovimiento, int id) {
            if (otroMovimiento.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (otroMovimiento).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/otroMovimiento/id
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