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
    public class TrasladosController : Controller {
        private readonly RhWebApiDbContext context;
        public TrasladosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/traslado
        [HttpGet]
        public IActionResult GetAll () {
            var traslados = context.Traslado.Select (t => new {
                Id = t.Id,
                    TrabajadorId = t.Trabajador.Id,
                    Trabajador = t.Trabajador.Nombre,
                    Fecha = t.Fecha,
                    // PuestoTrabajoOrigenId = t.CargoOrigen.Id,
                    // PuestoTrabajoOrigenId = t.CargoOrigen.Nombre,
                    // CargoDestinoId = t.CargoDestino.Id,
                    // CargoDestino = t.CargoDestino.Nombre,
            }).ToList ();
            return Ok (traslados);
        }

        // GET: recursos_humanos/traslado/Id
        [HttpGet ("{id}", Name = "GetTraslado")]
        public IActionResult GetbyId (int id) {
            var traslado = context.Traslado.FirstOrDefault (s => s.Id == id);

            if (traslado == null) {
                return NotFound ();
            }
            return Ok (traslado);

        }

        // POST recursos_humanos/traslado
        [HttpPost]
        public IActionResult POST ([FromBody] TrasladoDto trasladoDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == trasladoDto.TrabajadorId);
                var puesto = context.PuestoDeTrabajo.SingleOrDefault (p => p.CargoId == trasladoDto.CargoDestinoId && p.UnidadOrganizativaId == trasladoDto.UnidadOrganizativaId);

                if (trabajador == null) {
                    return NotFound ();
                }
                var traslado = new Traslado () {
                    TrabajadorId = trasladoDto.TrabajadorId,
                    Fecha = trasladoDto.Fecha,
                    CargoOrigenId = trabajador.PuestoDeTrabajo.CargoId,
                    CargoDestinoId = trasladoDto.CargoDestinoId,
                };
                context.Traslado.Add (traslado);
                trabajador.PuestoDeTrabajoId = puesto.Id;
                context.SaveChanges ();

                // var hist = context.HistoricoPuestoDeTrabajo.FirstOrDefault (s => s.TrabajadorId == trabajador.Id);
                // if (hist.FechaInicio != DateTime.Now && hist.FechaTerminado == null) {
                //     hist.FechaTerminado = DateTime.Now;
                // }

                var historico = new HistoricoPuestoDeTrabajo () {
                    TrabajadorId = trabajador.Id,
                    PuestoDeTrabajoId = trabajador.PuestoDeTrabajoId,
                    FechaInicio = DateTime.Now,
                };

                context.HistoricoPuestoDeTrabajo.Add (historico);
                context.SaveChanges ();
               // return new CreatedAtRouteResult ("GetTraslado", new { id = traslado.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/traslado/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Traslado traslado, int id) {
            if (traslado.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (traslado).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/traslado/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var traslado = context.Traslado.FirstOrDefault (s => s.Id == id);

            if (traslado.Id != id) {
                return NotFound ();
            }
            context.Traslado.Remove (traslado);
            context.SaveChanges ();
            return Ok (traslado);
        }
    }
}