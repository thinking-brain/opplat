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
            var dictaminador = context.DictaminadoresContratos.ToList ();
            var dictaminadores = new List<Trabajador> ();

            foreach (var item in dictaminador) {
                dictaminadores.Add (trabajadores.FirstOrDefault (s => s.Id == item.DictaminadorId));
            }
            return Ok (dictaminadores);
        }

        // GET: contratacion/DictContratos/Id
        [HttpGet ("{id}", Name = "Getdictaminador")]
        public IActionResult GetbyId (int id) {
            var dictaminador = context.DictaminadoresContratos.FirstOrDefault (s => s.DictaminadorId == id);

            if (dictaminador == null) {
                return NotFound ();
            }
            return Ok (dictaminador);
        }

        // POST contratacion/DictContratos
        [HttpPost]
        public IActionResult POST ([FromBody] List<int> DictaminadoresContratos) {
            if (ModelState.IsValid) {
                foreach (var item in DictaminadoresContratos) {
                    if (context.DictaminadoresContratos.FirstOrDefault (s => s.DictaminadorId == item) == null) {
                        var dictaminador = new DictaminadorContrato {
                        DictaminadorId = item
                        };
                        context.DictaminadoresContratos.Add (dictaminador);
                        context.SaveChanges ();
                    }
                }
                return Ok ();
            }
            return BadRequest (ModelState);
        }

        // PUT contratacion/DictContratos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] DictaminadorContrato dictaminador, int id) {
            if (dictaminador.DictaminadorId != id) {
                return BadRequest (ModelState);

            }
            context.Entry (dictaminador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/DictContratos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var dictaminador = context.DictaminadoresContratos.FirstOrDefault (s => s.DictaminadorId == id);

            if (dictaminador == null) {
                return NotFound ();
            }
            context.DictaminadoresContratos.Remove (dictaminador);
            context.SaveChanges ();
            return Ok (dictaminador);
        }
    }
}