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
    public class CategoriasOcupacionalesController : Controller {
        private readonly RhWebApiDbContext context;
        public CategoriasOcupacionalesController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/CategoriaOcupacional
        [HttpGet]
        public IEnumerable<CategoriaOcupacional> GetAll () {
            return context.CategoriaOcupacional.ToList ();
        }

        // GET: api/CategoriaOcupacional/Id
        [HttpGet ("{id}", Name = "GetCategoriaOcupacional")]
        public IActionResult GetbyId (int id) {
            var categoriaOcupacional = context.CategoriaOcupacional.FirstOrDefault (s => s.Id == id);

            if (categoriaOcupacional== null) {
                return NotFound ();
            }
            return Ok (categoriaOcupacional);

        }

        // POST api/CategoriaOcupacional
        [HttpPost]
        public IActionResult POST ([FromBody] CategoriaOcupacional categoriaOcupacional) {
            if (ModelState.IsValid) {
                context.CategoriaOcupacional.Add (categoriaOcupacional);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetCategoriaOcupacional", new { id = categoriaOcupacional.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/CategoriaOcupacional/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] CategoriaOcupacional categoriaOcupacional, int id) {
            if (categoriaOcupacional.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (categoriaOcupacional).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/CategoriaOcupacional/id
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