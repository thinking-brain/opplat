using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class CargosController : Controller {
        private readonly RhWebApiDbContext context;
        public CargosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/cargo
        [HttpGet]
        public IEnumerable<Cargo> GetAll () {
            return context.Cargo.ToList ();
        }

        // GET: api/cargo/Id
        [HttpGet ("{id}", Name = "GetCargo")]
        public IActionResult GetbyId (int id) {
            var cargo = context.Cargo.FirstOrDefault (s => s.Id == id);

            if (cargo == null) {
                return NotFound ();
            }
            return Ok (cargo);

        }

        // POST api/areas
        [HttpPost]
        public IActionResult POST ([FromBody] CargoDto cargoDto) {
            if (ModelState.IsValid) {
                var cargo = new Cargo () {
                    Id = cargoDto.Id,
                    Nombre = cargoDto.Nombre,
                    Sigla = cargoDto.Sigla,
                };
                context.Cargo.Add (cargo);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetCargo", new { id = cargo.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/areas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Cargo cargo, int id) {
            if (cargo.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (cargo).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/areas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var cargo = context.Cargo.FirstOrDefault (s => s.Id == id);

            if (cargo.Id != id) {
                return NotFound ();
            }
            context.Cargo.Remove (cargo);
            context.SaveChanges ();
            return Ok (cargo);
        }
    }
}