using System;
using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Dtos;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class ComiteContratacionController : Controller {
        private readonly ContratacionDbContext context;
        private readonly RhWebApiDbContext context_rh;

        public ComiteContratacionController (ContratacionDbContext context, RhWebApiDbContext context_rh) {
            this.context = context;
            this.context_rh = context_rh;
        }

        // GET contratos/AdminContratos
        [HttpGet]
        public IActionResult GetAll () {
            // var trabajadoresComiteCont = context_rh.Trabajador.Where (t => t.ComiteContratacionId != null).ToList ();

            return Ok ();
        }

        // GET: contratos/AdminContratos/Id
        [HttpGet ("{id}", Name = "GetTrabComiteCont")]
        public IActionResult GetbyId (int id) {

            return Ok ();
        }

        // POST contratos/AdminContratos
        [HttpPost]
        public IActionResult POST ([FromBody] ComiteContratacionDto comiteContratacionDto) {
            if (ModelState.IsValid) {
                foreach (var item in comiteContratacionDto.Trabajadores) {
                    var trab = context_rh.Trabajador.FirstOrDefault (s => s.Id == item);
                    if (trab != null) {
                        // trab.ComiteContratacionId = item;
                        context_rh.Trabajador.Add (trab);
                        context_rh.SaveChanges ();
                    }
                }
                return Ok ();
            }
            return BadRequest (ModelState);
        }

        // PUT contratos/trabComiteContratacion/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] ComiteContratacionDto comiteContratacionDto, int id) {
            var trab = context_rh.Trabajador.FirstOrDefault (s => s.Id == id);
            foreach (var item in comiteContratacionDto.Trabajadores) {
                // trab.ComiteContratacionId = item;
            }
            context.Entry (trab).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratos/trabComiteContratacion/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var trabComiteContratacion = context_rh.Trabajador.FirstOrDefault (s => s.Id == id);

            if (trabComiteContratacion == null) {
                return NotFound ();
            }
            context_rh.Trabajador.Remove (trabComiteContratacion);
            context.SaveChanges ();
            return Ok (trabComiteContratacion);
        }
    }
}