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
    public class EntradasController : Controller {
        private readonly RhWebApiDbContext context;
        public EntradasController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/EntradasController
        [HttpGet]
        public ActionResult GetAll () {

            var entrada = context.Entrada.Select (e => new {
                Id = e.Id,
                    Trabajador = e.Trabajador.Nombre,
                    Fecha = e.Fecha,
                    Cargo = e.Cargo.Nombre,
                    UnidadOrganizativa = e.UnidadOrganizativa.Nombre,
            }).ToList ();
            return Ok (entrada);
        }

        // GET: api/Entradas/Id
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

        // POST api/Entradas
        [HttpPost]
        public IActionResult POST ([FromBody] EntradaDto entradaDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == entradaDto.TrabajadorId);

                var puesto = context.PuestoDeTrabajo.SingleOrDefault (p => p.CargoId == entradaDto.CargoId && p.UnidadOrganizativaId == entradaDto.UnidadOrganizativaId);

                if (context.Entrada.Any (p => p.TrabajadorId == entradaDto.TrabajadorId)) {
                    return BadRequest ($"Error al trabajador {trabajador.Nombre +" "+ trabajador.Apellidos}, ya se le ha dado entrada");
                } else {
                    var entrada = new Entrada () {
                        TrabajadorId = entradaDto.TrabajadorId,
                        CargoId = entradaDto.CargoId,
                        Fecha = entradaDto.Fecha,
                        UnidadOrganizativaId = entradaDto.UnidadOrganizativaId,
                    };
                    context.Entrada.Add (entrada);
                    context.SaveChanges ();

                    trabajador.PuestoDeTrabajoId = puesto.Id;
                    puesto.CantidadPorPlantilla++;
                    trabajador.EstadoTrabajador = "Activo";
                    context.Entry (trabajador).State = EntityState.Modified;
                    context.SaveChanges ();

                    // if (context.PuestoDeTrabajo.Any (p => p.CargoId == entradaDto.CargoId)) {
                    //     puesto.CantidadPorPlantilla++;
                    //     context.Entry (puesto).State = EntityState.Modified;
                    // } else {
                    //     puesto = new PuestoDeTrabajo () {
                    //         CantidadPorPlantilla = 1,
                    //         CargoId = entradaDto.CargoId,
                    //     };
                    //     context.Add (puesto);
                    // }

                    return new CreatedAtRouteResult ("GetEntrada", new { id = entradaDto.Id });
                }
            }
            return BadRequest (ModelState);
        }

        // PUT api/Entradas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] EntradaDto entradaDto, int id) {
            if (entradaDto.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (entradaDto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/Entradas/id
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