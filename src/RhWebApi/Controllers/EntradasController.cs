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
    public class EntradasController : Controller {
        private readonly RhWebApiDbContext context;
        public EntradasController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/EntradasController
        [HttpGet]
        public ActionResult GetAll () {

            var entrada = context.Entrada.Select (e => new {
                Id = e.Id,
                    Trabajador = e.Trabajador.Nombre,
                    Fecha = e.Fecha,
                    Cargo = e.Cargo.Nombre,
                    UnidadOrganizativa = e.UnidadOrganizativa.Nombre,
            }).ToList ();
            if (entrada == null) {
                return NotFound ();
            } else {
                return Ok (entrada);
            }
        }

        // GET: recursos_humanos/Entradas/Id
        [HttpGet ("{id}", Name = "GetEntrada")]
        public IActionResult GetbyId (int id) {
            var entrada = context.Entrada.FirstOrDefault (s => s.Id == id);

            if (entrada == null) {
                return NotFound ();
            }

            var result = context.Entrada.Where (s => s.Id == id).Select (e => new {
                Id = e.Id,
                    Trabajador = e.Trabajador.Nombre,
                    Fecha = e.Fecha,
                    Cargo = e.Cargo.Nombre,
                    UnidadOrganizativa = e.UnidadOrganizativa.Nombre,
            });
            return Ok (result);
        }

        // POST recursos_humanos/Entradas
        [HttpPost]
        public IActionResult POST ([FromBody] EntradaDto entradaDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == entradaDto.TrabajadorId);
                if (trabajador != null) {
                    var puesto = context.PuestoDeTrabajo.FirstOrDefault (p => p.CargoId == entradaDto.CargoId && p.UnidadOrganizativaId == entradaDto.UnidadOrganizativaId);
                    if (puesto == null) {
                        var puestoNew = new PuestoDeTrabajo () {
                        CargoId = entradaDto.CargoId,
                        UnidadOrganizativaId = entradaDto.UnidadOrganizativaId,
                        };
                        context.PuestoDeTrabajo.Add (puestoNew);
                        context.SaveChanges ();
                    }
                    var entrada = new Entrada () {
                        TrabajadorId = entradaDto.TrabajadorId,
                        CargoId = entradaDto.CargoId,
                        Fecha = entradaDto.Fecha,
                        UnidadOrganizativaId = entradaDto.UnidadOrganizativaId,
                    };
                    context.Entrada.Add (entrada);
                    context.SaveChanges ();

                    puesto = context.PuestoDeTrabajo.FirstOrDefault (p => p.CargoId == entradaDto.CargoId && p.UnidadOrganizativaId == entradaDto.UnidadOrganizativaId);
                    puesto.PlantillaOcupada++;
                    trabajador.PuestoDeTrabajoId = puesto.Id;
                    trabajador.EstadoTrabajador = Estados.Activo;
                    context.Entry (trabajador).State = EntityState.Modified;

                    var historico = new HistoricoPuestoDeTrabajo () {
                        TrabajadorId = trabajador.Id,
                        PuestoDeTrabajoId = trabajador.PuestoDeTrabajoId,
                        FechaInicio = DateTime.Now,
                    };

                    context.HistoricoPuestoDeTrabajo.Add (historico);
                    context.SaveChanges ();
                    return new CreatedAtRouteResult ("GetEntrada", new { id = entradaDto.Id });
                } else {
                    return BadRequest ($"El trabajador no está en el Sistema debe agregarlo");
                }
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/Entradas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] EntradaDto entradaDto, int id) {
            if (entradaDto.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (entradaDto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/Entradas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var entrada = context.Entrada.FirstOrDefault (s => s.Id == id);

            if (entrada.Id != id) {
                return NotFound ();
            }
            context.Entrada.Remove (entrada);
            context.SaveChanges ();
            return Ok (entrada);
        }
    }
}