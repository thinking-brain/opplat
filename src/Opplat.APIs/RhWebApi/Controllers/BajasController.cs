﻿using System;
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
    public class BajasController : Controller {
        private readonly RhWebApiDbContext context;
        public BajasController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/baja
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

        // GET: recursos_humanos/baja/Id
        [HttpGet ("{id}", Name = "GetBaja")]
        public IActionResult GetbyId (int id) {
            var baja = context.Baja.FirstOrDefault (s => s.Id == id);

            if (baja == null) {
                return NotFound ();
            }
            return Ok (baja);
        }

        // POST recursos_humanos/baja
        [HttpPost]
        public IActionResult POST ([FromBody] BajaDto bajaDto) {
            if (ModelState.IsValid) {
                var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == bajaDto.TrabajadorId);
                if (trabajador == null) {
                    return NotFound ();
                } else if (trabajador.EstadoTrabajador == Estados.Baja) {
                    return BadRequest ($"El trabajador ya esta de baja");
                } else {
                    var baja = new Baja () {
                        TrabajadorId = bajaDto.TrabajadorId,
                        Fecha = bajaDto.Fecha,
                        CausaDeBaja = bajaDto.CausaDeBaja,
                    };
                    context.Baja.Add (baja);
                    context.SaveChanges ();
                    trabajador.EstadoTrabajador = Estados.Baja;
                    // trabajador.PuestoDeTrabajo.PlantillaOcupada--;
                    trabajador.PuestoDeTrabajoId = null;
                    context.SaveChanges ();

                    return new CreatedAtRouteResult ("GetBaja", new { id = baja.Id });
                }

            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/baja/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Baja baja, int id) {
            if (baja.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (baja).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/baja/id
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
        // GET: recursos_humanos/baja/CausaDeBaja
        [HttpGet ("/recursos_humanos/Baja/CausaDeBaja")]
        public IActionResult GetAllCausaDeBaja () {
            var causaDeBaja = new List<dynamic> () {
                new { Id = CausaDeBaja.SinDefinir, Nombre = "Sin Definir" },
                new { Id = CausaDeBaja.Salarial, Nombre = CausaDeBaja.Salarial.ToString () },
                new { Id = CausaDeBaja.Jubilación, Nombre = CausaDeBaja.Jubilación.ToString () },
                new { Id = CausaDeBaja.Fallecimiento, Nombre = CausaDeBaja.Fallecimiento.ToString () },
                new { Id = CausaDeBaja.InvalidezTotal, Nombre = "Invalidez Total" },
                new { Id = CausaDeBaja.InvalidezParcial, Nombre =  "Invalidez Parcial"},
                new { Id = CausaDeBaja.PrivacionDeLibertad, Nombre = "Privación de Libertad" },
                new { Id = CausaDeBaja.PasoAFormasNoEstatales, Nombre = "Paso a Formas no Estatales" },
                new { Id = CausaDeBaja.ProcesoDeDisponibilidad, Nombre = "Proceso de Disponibilidad" },
                new { Id = CausaDeBaja.SancionAdministrativa, Nombre = "Sanción Administrativa" },
                new { Id = CausaDeBaja.SalidaDelPais, Nombre = "Salida del País" },
                new { Id = CausaDeBaja.OtrasCausas, Nombre = "Otras Causas" },
            };
            return Ok (causaDeBaja);
        }
    }
}