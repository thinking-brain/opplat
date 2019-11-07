using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class BajasController : Controller {
        private readonly RhWebApiDbContext context;
        public BajasController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/baja
        [HttpGet]
        public IEnumerable<BajaDto> GetAll () {
            var baja = context.Baja.Select (s => new BajaDto {
                Id = s.Id,
                    TrabajadorId = s.TrabajadorId,
                    Trabajador = s.Trabajador.Nombre,
                    CausaDeBaja = s.CausaDeBaja,
                    Fecha = s.Fecha,
            });
            return baja;
        }

        // GET: api/baja/Id
        [HttpGet ("{id}", Name = "GetBaja")]
        public IActionResult GetbyId (int id) {
            var baja = context.Baja.FirstOrDefault (s => s.Id == id);

            if (baja == null) {
                return NotFound ();
            }
            return Ok (baja);

        }

        // POST api/baja
        [HttpPost]
        public IActionResult POST ([FromBody] BajaDto bajaDto) {
            if (ModelState.IsValid) {
                var baja = new Baja () {
                    TrabajadorId = bajaDto.TrabajadorId,
                    Fecha = bajaDto.Fecha,
                    CausaDeBaja = bajaDto.CausaDeBaja,
                };
                context.Baja.Add (baja);
                context.SaveChanges ();

                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == bajaDto.TrabajadorId);
                trabajador.EstadoTrabajador = "Baja";
                trabajador.PuestoDeTrabajoId = null;
                context.SaveChanges ();

                return new CreatedAtRouteResult ("GetBaja", new { id = baja.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/baja/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Baja baja, int id) {
            if (baja.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (baja).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/baja/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var baja = context.Baja.FirstOrDefault (s => s.Id == id);

            if (baja.Id != id) {
                return NotFound ();
            }
            context.Baja.Remove (baja);
            context.SaveChanges ();
            return Ok (baja);
        }
    }
}