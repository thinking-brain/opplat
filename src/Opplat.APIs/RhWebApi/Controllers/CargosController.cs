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
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class CargosController : ControllerBase {
        private readonly RhWebApiDbContext context;
        public CargosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/cargo
        [HttpGet]
        public IEnumerable<Cargo> GetAll () {
            return context.Cargo.ToList ();
        }

        // GET: recursos_humanos/cargo/Id
        [HttpGet ("{id}", Name = "GetCargo")]
        public IActionResult GetbyId (int id) {
            var cargo = context.Cargo.FirstOrDefault (s => s.Id == id);

            if (cargo == null) {
                return NotFound ();
            }
            return Ok (cargo);

        }

        // POST recursos_humanos/areas
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

        // PUT recursos_humanos/areas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Cargo cargo, int id) {
            if (cargo.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (cargo).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/areas/id
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