using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class TrasladosController : Controller {
        private readonly RhWebApiDbContext context;
        public TrasladosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/traslados
        [HttpGet]
        public IActionResult GetAll () {
            var traslados = context.Traslado.Select (t => new {
                Id = t.Id,
                    TrabajadorId = t.Trabajador.Id,
                    Trabajador = t.Trabajador.Nombre,
                    Fecha = t.Fecha,
                    PuestoTrabajoOrigenId = t.CargoOrigen.Id,
                    PuestoTrabajoOrigen = t.CargoOrigen.Nombre,
                    CargoDestinoId = t.CargoDestino.Id,
                    CargoDestino = t.CargoDestino.Nombre,
            }).ToList ();
            return Ok (traslados);
        }

        // GET: recursos_humanos/traslados/Id
        [HttpGet ("{id}", Name = "GetTraslado")]
        public IActionResult GetbyId (int id) {
            var traslado = context.Traslado.FirstOrDefault (s => s.Id == id);

            if (traslado == null) {
                return NotFound ();
            }
            return Ok (traslado);

        }

        // POST recursos_humanos/traslados
        [HttpPost]
        public IActionResult POST ([FromBody] TrasladoDto trasladoDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == trasladoDto.TrabajadorId);
                var puesto = context.PuestoDeTrabajo.FirstOrDefault (p => p.CargoId == trasladoDto.CargoDestinoId && p.UnidadOrganizativaId == trasladoDto.UnidOrgDestinoId);

                if (trabajador == null) {
                    return NotFound ();
                }
                var traslado = new Traslado () {
                    Fecha = trasladoDto.Fecha,
                    TrabajadorId = trasladoDto.TrabajadorId,
                    CargoOrigenId = trasladoDto.CargoOrigenId,
                    CargoDestinoId = trasladoDto.CargoDestinoId,
                    UnidOrgOrigenId = trasladoDto.UnidOrgOrigenId,
                    UnidOrgDestinoId = trasladoDto.UnidOrgDestinoId
                };
                context.Traslado.Add (traslado);
                context.SaveChanges ();

                //Crear nuevo puesto de trabajo por si no existe
                if (puesto != null) {
                    trabajador.PuestoDeTrabajoId = puesto.Id;
                    puesto.PlantillaOcupada++;
                } else {
                    var puestoNew = new PuestoDeTrabajo () {
                        CargoId = trasladoDto.CargoDestinoId,
                        UnidadOrganizativaId = trasladoDto.UnidOrgDestinoId,
                        PlantillaOcupada = +1
                    };
                    context.PuestoDeTrabajo.Add (puestoNew);
                    trabajador.PuestoDeTrabajoId = puestoNew.Id;
                    context.Entry (trabajador).State = EntityState.Modified;
                }
                context.SaveChanges ();

                var hist = context.HistoricoPuestoDeTrabajo.FirstOrDefault (s => s.TrabajadorId == trabajador.Id);
                if (hist.FechaInicio != DateTime.Now && hist.FechaTerminado == null) {
                    hist.FechaTerminado = DateTime.Now;
                }
                var historico = new HistoricoPuestoDeTrabajo () {
                    TrabajadorId = trabajador.Id,
                    PuestoDeTrabajoId = trabajador.PuestoDeTrabajoId,
                    FechaInicio = DateTime.Now,
                };

                context.HistoricoPuestoDeTrabajo.Add (historico);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTraslado", new { id = traslado.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/traslados/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Traslado traslado, int id) {
            if (traslado.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (traslado).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/traslados/id
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