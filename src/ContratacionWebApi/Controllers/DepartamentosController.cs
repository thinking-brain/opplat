using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class DepartamentosController : Controller {
        private readonly ContratacionDbContext context;
        public DepartamentosController (ContratacionDbContext context) {
            this.context = context;
        }
        // GET contratacion/Departamentos
        [HttpGet]
        public IEnumerable<Departamento> GetAll () {
            return context.Departamentos.ToList ();
        }

        // GET: contratacion/Departamentos/Id
        [HttpGet ("{id}", Name = "GetDeparatmentos")]
        public IActionResult GetbyId (int id) {
            var departamento = context.Departamentos.FirstOrDefault (s => s.Id == id);

            if (departamento == null) {
                return NotFound ();
            }
            return Ok (departamento);
        }

        // POST contratacion/Departamentos
        [HttpPost]
        public IActionResult POST ([FromBody] Departamento departamento) {
            if (ModelState.IsValid) {
                var d = context.Departamentos.FirstOrDefault (x => x.Nombre == departamento.Nombre);
                if (d != null) {
                    return BadRequest ("Ya el departamento " + departamento.Nombre + " está creado");
                }
                context.Departamentos.Add (departamento);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetDeparatmentos", new { id = departamento.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT contratacion/departamento/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Departamento departamento, int id) {
            if (departamento.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (departamento).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/departamento/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var departamento = context.Departamentos.FirstOrDefault (s => s.Id == id);

            if (departamento.Id != id) {
                return NotFound ();
            }
            context.Departamentos.Remove (departamento);
            context.SaveChanges ();
            return Ok (departamento);
        }
    }
}