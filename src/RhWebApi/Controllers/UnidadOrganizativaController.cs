using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

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
    //     // GET: recursos_humanos/UnidadOrgaSexo/{id},{sexo}
    //    [HttpGet ("{id}")]
    //     public IActionResult GetByUnidadOrgSexo (int id, Sexo sexo) {
    //         var trabajadores = context.Trabajador
    //             .Include (t => t.PuestoDeTrabajo)
    //             .Where (s => s.PuestoDeTrabajo.UnidadOrganizativaId == id && s.Sexo == sexo)
    //             .Select (s => new {
    //                 Id = s.Id,
    //                     Nombre = s.Nombre + " " + s.Apellidos,
    //                     Cargo = s.PuestoDeTrabajo.Cargo.Nombre,
    //             })
    //             .ToList ();
    //         return Ok (trabajadores);
    //     }
    }
}