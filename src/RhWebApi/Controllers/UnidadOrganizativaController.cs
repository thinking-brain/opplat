using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class UnidadOrganizativaController : Controller {
        private readonly RhWebApiDbContext context;
        public UnidadOrganizativaController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET: api/UnidadOrganizativa/Id
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
    }
}