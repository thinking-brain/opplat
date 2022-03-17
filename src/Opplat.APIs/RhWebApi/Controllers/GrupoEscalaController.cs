using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;
using RhWebApi.Dtos;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class GrupoEscalaController : ControllerBase {
        private readonly RhWebApiDbContext context;
        public GrupoEscalaController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/GrupoEscala
        [HttpGet]
        public IEnumerable<GrupoEscala> GetAll () {
            return context.GrupoEscala.ToList ();
        }

        // GET: recursos_humanos/GrupoEscala/Id
        [HttpGet ("{id}", Name = "GetGrupoEscala")]
        public IActionResult GetbyId (int id) {
            var grupoEscala = context.GrupoEscala.FirstOrDefault (s => s.Id == id);

            if (grupoEscala == null) {
                return NotFound ();
            }
            return Ok (grupoEscala);

        }

        // POST recursos_humanos/GrupoEscala
        [HttpPost]
        public IActionResult POST ([FromBody] GrupoEscalaDto grupoEscalaDto) {
            if (ModelState.IsValid) {
                var grupoEscala = new GrupoEscala ();
                grupoEscala.SalarioDiferenciado=grupoEscalaDto.SalarioDiferenciado;
                grupoEscala.SalarioEscala=grupoEscalaDto.SalarioEscala;
                grupoEscala.CategoriaOcupacionalId=grupoEscala.CategoriaOcupacionalId;
                grupoEscala.Codigo=grupoEscalaDto.Codigo;
                context.GrupoEscala.Add (grupoEscala);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetGrupoEscala", new { id = grupoEscala.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/grupoEscala/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] GrupoEscala grupoEscala, int id) {
            if (grupoEscala.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (grupoEscala).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/grupoEscala/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var grupoEscala = context.GrupoEscala.FirstOrDefault (s => s.Id == id);

            if (grupoEscala.Id != id) {
                return NotFound ();
            }
            context.GrupoEscala.Remove (grupoEscala);
            context.SaveChanges ();
            return Ok (grupoEscala);
        }
    }
}