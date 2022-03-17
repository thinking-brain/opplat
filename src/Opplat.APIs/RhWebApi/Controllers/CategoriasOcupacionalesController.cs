using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class CategoriasOcupacionalesController : ControllerBase {
        private readonly RhWebApiDbContext context;
        public CategoriasOcupacionalesController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/CategoriaOcupacional
        [HttpGet]
        public IEnumerable<CategoriaOcupacional> GetAll () {
            return context.CategoriaOcupacional.ToList ();
        }

        // GET: recursos_humanos/CategoriaOcupacional/Id
        [HttpGet ("{id}", Name = "GetCategoriaOcupacional")]
        public IActionResult GetbyId (int id) {
            var categoriaOcupacional = context.CategoriaOcupacional.FirstOrDefault (s => s.Id == id);

            if (categoriaOcupacional== null) {
                return NotFound ();
            }
            return Ok (categoriaOcupacional);

        }

        // POST recursos_humanos/CategoriaOcupacional
        [HttpPost]
        public IActionResult POST ([FromBody] CategoriaOcupacional categoriaOcupacional) {
            if (ModelState.IsValid) {
                context.CategoriaOcupacional.Add (categoriaOcupacional);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetCategoriaOcupacional", new { id = categoriaOcupacional.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/CategoriaOcupacional/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] CategoriaOcupacional categoriaOcupacional, int id) {
            if (categoriaOcupacional.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (categoriaOcupacional).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/CategoriaOcupacional/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var categoriaOcupacional = context.CategoriaOcupacional.FirstOrDefault (s => s.Id == id);

            if (categoriaOcupacional.Id != id) {
                return NotFound ();
            }
            context.CategoriaOcupacional.Remove (categoriaOcupacional);
            context.SaveChanges ();
            return Ok (categoriaOcupacional);
        }
    }
}