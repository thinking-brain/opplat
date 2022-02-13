using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class UnidadOrganizativaController : Controller {
        private readonly RhWebApiDbContext context;
        public UnidadOrganizativaController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET: recursos_humanos/UnidadOrganizativa
        [HttpGet]
        public IActionResult GetAll () {
            var unidadOrganizativa = context.UnidadOrganizativa.Select (t => new {
                Id = t.Id,
                    Nombre = t.Nombre,
            });
            if (unidadOrganizativa == null) {
                return NotFound ();
            }
            return Ok (unidadOrganizativa);
        }

        // GET: recursos_humanos/UnidadOrganizativa/Id
        [HttpGet ("{id}")]
        public IActionResult GetByUnidadOrganizativa (int id) {
            var trabajadores = context.Trabajador
                .Include (t => t.PuestoDeTrabajo)
                .Where (s => s.PuestoDeTrabajo.UnidadOrganizativaId == id)
                .Select (s => new {
                    Id = s.Id,
                        Nombre = s.Nombre + " " + s.Apellidos,
                        Cargo = s.PuestoDeTrabajo.Cargo.Nombre,
                })
                .ToList ();
            return Ok (trabajadores);
        }

        // GET: recursos_humanos/UnidadOrgaIdSexo/{id},{sexo}
        [HttpGet ("/recursos_humanos/UnidadOrgIdSexo/Id")]
        public IActionResult GetByUnidadOrgSexo (int id, Sexo sexo) {
            var trabajadores = context.Trabajador
                .Include (t => t.PuestoDeTrabajo)
                .Where (s => s.PuestoDeTrabajo.UnidadOrganizativaId == id && s.Sexo == sexo)
                .Select (s => new {
                    Id = s.Id,
                        Nombre = s.Nombre + " " + s.Apellidos,
                        Cargo = s.PuestoDeTrabajo.Cargo.Nombre,
                })
                .ToList ();
            return Ok (trabajadores);
        }
       
        // GET: recursos_humanos/UnidadOrganizativa
        [HttpGet ("/recursos_humanos/UnidadOrgPlantCubierta")]
        public IActionResult GetAllPlantillaCubierta () {
            var trabajadores = context.Trabajador
                .Include (m => m.PuestoDeTrabajo)
                 .GroupBy(m => m.PuestoDeTrabajo)
                .Select (m => new {
                        plantCubierta = m.Count(),
                }).ToList ();
            var result = context.PuestoDeTrabajo
                .Select (s => new {
                    id=s.UnidadOrganizativaId,
                    unidadOrganizativa = s.UnidadOrganizativa.Nombre,
                    planTillaCubierta=trabajadores.Sum(p=>p.plantCubierta)
                });
            return Ok (trabajadores);
        }
    }
}