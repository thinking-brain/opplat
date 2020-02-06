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

        // GET recursos_humanos/bolsa
        [HttpGet]
        public IEnumerable<Bolsa> GetAll () {
            return context.Bolsa.ToList ();
        }

        // // GET: recursos_humanos/bolsa/Id
        // [HttpGet ("{id}", Name = "GetBolsa")]
        // public IActionResult GetbyId (int id) {

        //     return Ok ();
        // }

        // POST recursos_humanos/bolsa
        [HttpPost]
        public IActionResult POST (int id) {
             var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == id);
            if (trabajador == null) {
                return NotFound ();
            }
            trabajador.EstadoTrabajador=Estados.Descartado;
            context.Entry (trabajador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // // PUT recursos_humanos/bolsa/id
        // [HttpPut ("{id}")]
        // public IActionResult PUT (int id) {
        //     return Ok ();
        // }

        // // DELETE recursos_humanos/bolsa/id
        // [HttpDelete ("{id}")]
        // public IActionResult Delete (int id) {
        //     var bolsa = context.Bolsa.FirstOrDefault (s => s.Id == id);

        //     if (bolsa.Id != id) {
        //         return NotFound ();
        //     }
        //     context.Bolsa.Remove (bolsa);
        //     context.SaveChanges ();
        //     return Ok (bolsa);
        // }
    }
}