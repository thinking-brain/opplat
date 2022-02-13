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
    public class FuncionesController : Controller {
        private readonly RhWebApiDbContext context;
        public FuncionesController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/Funciones
        [HttpGet]
        public IEnumerable<Funciones> GetAll () {
            return context.Funciones.ToList ();
        }

        // GET: recursos_humanos/Funciones/Id
        [HttpGet ("{id}", Name = "GetFunciones")]
        public IActionResult GetbyId (int id) {
            var funcion = context.Funciones.FirstOrDefault (s => s.Id == id);

            if (funcion == null) {
                return NotFound ();
            }
            return Ok (funcion);

        }

        // POST recursos_humanos/Funciones
        [HttpPost]
        public IActionResult POST ([FromBody] Funciones funcion) {
            if (ModelState.IsValid) {
                context.Funciones.Add (funcion);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetFunciones", new { id = funcion.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/Funciones/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Funciones funcion, int id) {
            if (funcion.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (funcion).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/Funciones/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var funcion = context.Funciones.FirstOrDefault (s => s.Id == id);

            if (funcion.Id != id) {
                return NotFound ();
            }
            context.Funciones.Remove (funcion);
            context.SaveChanges ();
            return Ok (funcion);
        }
    }
}