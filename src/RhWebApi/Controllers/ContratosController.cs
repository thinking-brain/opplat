using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ContratosController : Controller {
        private readonly RhWebApiDbContext context;
        public ContratosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/Contrato
        [HttpGet]
        public IEnumerable<Contrato> GetAll () {
            return context.Contrato.ToList ();
        }

        // GET: api/Contrato/Id
        [HttpGet ("{id}", Name = "GetContrato")]
        public IActionResult GetbyId (int id) {
            var contrato = context.Contrato.FirstOrDefault (s => s.Id == id);

            if (contrato == null) {
                return NotFound ();
            }
            return Ok (contrato);

        }

        // POST api/Contrato
        [HttpPost]
        public IActionResult POST ([FromBody] Contrato contrato) {
            if (ModelState.IsValid) {
                context.Contrato.Add (contrato);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetContrato", new { id = contrato.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/areas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Contrato contrato, int id) {
            if (contrato.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (contrato).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/areas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var contrato = context.Contrato.FirstOrDefault (s => s.Id == id);

            if (contrato.Id != id) {
                return NotFound ();
            }
            context.Contrato.Remove (contrato);
            context.SaveChanges ();
            return Ok (contrato);
        }
    }
}