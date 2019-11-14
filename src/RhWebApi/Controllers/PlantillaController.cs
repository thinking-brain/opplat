using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class PlantillaController : Controller {
        private readonly RhWebApiDbContext context;
        public PlantillaController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/PlantillaController
        [HttpGet]
        public ActionResult GetAll () {

            var plantilla = context.Plantilla.Select (p => new {
                Id = p.Id,
                    UnidadOrganizativa = p.UnidadOrganizativa.Nombre,
                    Cargo = p.Cargo.Nombre,
                    Plantilla = p.PlantillaAprobada,
            }).ToList ();
            if (plantilla == null) {
                return NotFound ();
            } else {
                return Ok (plantilla);
            }
        }

        // GET: recursos_humanos/PlantillaByUnidadOrg/Id
        [HttpGet ("/recursos_humanos/PlantillaByUnidadOrg/{Id}")]
        public IActionResult GetbyUnidOrg (int id) {
   
               var plantilla = context.Plantilla.Where(p=>p.UnidadOrganizativaId==id).Select (p => new {
                Id = p.Id,
                    UnidadOrganizativa = p.UnidadOrganizativa.Nombre,
                    Cargo = p.Cargo.Nombre,
                    Plantilla = p.PlantillaAprobada,
            }).ToList ();
            if (plantilla == null) {
                return NotFound ();
            } else {
                return Ok (plantilla);
            }
        }

        // POST recursos_humanos/Plantilla
        [HttpPost]
        public IActionResult POST ([FromBody] PlantillaDto plantillaDto) {
            if (ModelState.IsValid) {
                var plantilla = new Plantilla () {
                    UnidadOrganizativaId = plantillaDto.UnidadOrganizativaId,
                    CargoId = plantillaDto.CargoId,
                    PlantillaAprobada = plantillaDto.PlantillaAprobada,
                };
                context.Plantilla.Add (plantilla);
                context.SaveChanges ();

                return new CreatedAtRouteResult ("GetPlantPorUnidOrgId", new { id = plantillaDto.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/Plantilla/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] PlantillaDto plantillaDto, int id) {
            if (plantillaDto.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (plantillaDto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/Plantilla/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var plantilla = context.Plantilla.FirstOrDefault (s => s.Id == id);

            if (plantilla.Id != id) {
                return NotFound ();
            }
            context.Plantilla.Remove (plantilla);
            context.SaveChanges ();
            return Ok (plantilla);
        }
    }
}