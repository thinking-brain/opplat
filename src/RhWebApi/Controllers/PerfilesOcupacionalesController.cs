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
    public class PerfilesOcupacionalesController : Controller {
        private readonly RhWebApiDbContext context;
        public PerfilesOcupacionalesController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/PerfilesOcupacionales
        [HttpGet]
        public IEnumerable<PerfilOcupacional> GetAll () {
            return context.PerfilOcupacional.ToList ();
        }

        // GET: recursos_humanos/PerfilesOcupacionales/Id
        [HttpGet ("{id}", Name = "GetPerfilOcupacional")]
        public IActionResult GetbyId (int id) {
            var perfilOcupacional = context.PerfilOcupacional.FirstOrDefault (s => s.Id == id);

            if (perfilOcupacional == null) {
                return NotFound ();
            }
            return Ok (perfilOcupacional);
        }

        // POST recursos_humanos/PerfilesOcupacionales
        [HttpPost]
        public IActionResult POST ([FromBody] PerfilOcupacional PerfilOcupacional) {
           if (ModelState.IsValid) {
                context.PerfilOcupacional.Add (PerfilOcupacional);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetContrato", new { id = PerfilOcupacional.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/PerfilesOcupacionales/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] PerfilOcupacional PerfilOcupacional, int id) {
            if (PerfilOcupacional.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (PerfilOcupacional).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/PerfilesOcupacionales/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var perfilOcupacional = context.PerfilOcupacional.FirstOrDefault (s => s.Id == id);

            if (perfilOcupacional.Id != id) {
                return NotFound ();
            }
            context.PerfilOcupacional.Remove (perfilOcupacional);
            context.SaveChanges ();
            return Ok (perfilOcupacional);
        }
    }
}