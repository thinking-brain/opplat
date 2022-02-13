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
    public class BolsasController : Controller {
        private readonly RhWebApiDbContext context;
        public BolsasController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/bolsas
        [HttpGet]
        public IEnumerable<Bolsa> GetAll () {
            return context.Bolsa.ToList ();
        }

        // GET: recursos_humanos/bolsas/Id
        [HttpGet ("{id}", Name = "GetBolsa")]
        public IEnumerable<Bolsa> GetbyId (int id) {
            return context.Bolsa.Where (s => s.Id == id).ToList ();
        }

        // POST recursos_humanos/bolsas
        [HttpPost]
        public IActionResult POST (Bolsa bolsa) {
            return Ok ();
        }

        // PUT recursos_humanos/bolsas/id
        [HttpPut ("{id}")]
        public IActionResult PUT (int id) {
            var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == id);
            if (trabajador == null) {
                return NotFound ();
            }
            trabajador.EstadoTrabajador = Estados.Descartado;
            context.Entry (trabajador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/bolsas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var bolsa = context.Bolsa.FirstOrDefault (s => s.Id == id);
            if (bolsa.Id != id) {
                return NotFound ();
            }
            context.Bolsa.Remove (bolsa);
            context.SaveChanges ();
            return Ok (bolsa);
        }
    }
}