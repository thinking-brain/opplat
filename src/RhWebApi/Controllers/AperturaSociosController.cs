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
    public class AperturaSociosController : Controller {
        private readonly RhWebApiDbContext context;
        public AperturaSociosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/AperturaSocio
        [HttpGet]
        public IActionResult GetAll () {
            var aperturaSocio = context.AperturaSocio
                .Select (s => new {
                    Id = s.Id,
                        Fecha = s.Fecha.ToString ("dd MMMM yyyy"),
                        CantTrabajadores = s.CantTrabajadores,
                        NumeroAcuerdo = s.NumeroAcuerdo,
                        Cerrada = s.Cerrada,
                }).ToList ();
            if (aperturaSocio == null) {
                return NotFound ();
            }
            return Ok (aperturaSocio);
        }

        // GET: recursos_humanos/AperturaSocio/Id
        [HttpGet ("{id}", Name = "GetApertura")]
        public IActionResult GetbyId (int id) {
            var aperturaSocio = context.AperturaSocio.FirstOrDefault (s => s.Id == id);
            if (aperturaSocio == null) {
                return NotFound ();
            }
            return Ok (aperturaSocio);
        }

        // POST recursos_humanos/AperturaSocio
        [HttpPost]
        public IActionResult POST ([FromBody] AperturaSocio aperturaSocio) {
            if (ModelState.IsValid) {
                var apertura = new AperturaSocio () {
                    Id = aperturaSocio.Id,
                };
                context.AperturaSocio.Add (apertura);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetApertura", new { id = apertura.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/areas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] AperturaSocio aperturaSocio, int id) {
            if (aperturaSocio.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (aperturaSocio).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/AperturaSocio/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var aperturaSocio = context.AperturaSocio.FirstOrDefault (s => s.Id == id);

            if (aperturaSocio.Id != id) {
                return NotFound ();
            }
            context.AperturaSocio.Remove (aperturaSocio);
            context.SaveChanges ();
            return Ok (aperturaSocio);
        }
    }
}