using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class DictContratosController : Controller {
        private readonly ContratacionDbContext context;
        private readonly RhWebApiDbContext context_rh;

        public DictContratosController (ContratacionDbContext context, RhWebApiDbContext context_rh) {
            this.context = context;
            this.context_rh = context_rh;
        }

        // GET contratacion/DictContratos
        [HttpGet]
        public IActionResult GetAll () {
            var trabajadores = context_rh.Trabajador.ToList ();
            var dictaminador = context.Dictaminadores.ToList ();
            var dictaminadores = new List<Trabajador> ();

            foreach (var item in dictaminador) {
                dictaminadores.Add (trabajadores.FirstOrDefault (s => s.Id == item.TrabajadorId));
            }
            return Ok (dictaminadores);
        }

        // GET: contratacion/DictContratos/Id
        [HttpGet ("{id}", Name = "Getdictaminador")]
        public IActionResult GetbyId (int id) {
            var dictaminador = context.Dictaminadores.FirstOrDefault (s => s.Id == id);

            if (dictaminador == null) {
                return NotFound ();
            }
            return Ok (dictaminador);
        }

        // POST contratacion/DictContratos
        [HttpPost]
        public IActionResult POST ([FromBody] Dictaminador dictaminador) {
            if (ModelState.IsValid) {
                var dictaminadorCont = context.Dictaminadores.FirstOrDefault (s => s.TrabajadorId == dictaminador.TrabajadorId);
                if (dictaminadorCont != null) {
                    return BadRequest ($"El Trabajador ya es Dictaminador de Contrato");
                } else {
                    context.Dictaminadores.Add (dictaminador);
                    context.SaveChanges ();
                    return new CreatedAtRouteResult ("GetDictContrato", new { id = dictaminador.Id });
                }
            }
            return BadRequest (ModelState);
        }

        // PUT contratacion/DictContratos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Dictaminador dictaminador, int id) {
            if (dictaminador.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (dictaminador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/DictContratos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var dictaminador = context.Dictaminadores.FirstOrDefault (s => s.Id == id);

            if (dictaminador.Id != id) {
                return NotFound ();
            }
            context.Dictaminadores.Remove (dictaminador);
            context.SaveChanges ();
            return Ok (dictaminador);
        }
    }
}