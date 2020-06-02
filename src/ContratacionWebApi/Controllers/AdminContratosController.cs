using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class AdminContratosController : Controller {
        private readonly ContratacionDbContext context;
        private readonly RhWebApiDbContext context_rh;

        public AdminContratosController (ContratacionDbContext context, RhWebApiDbContext context_rh) {
            this.context = context;
            this.context_rh = context_rh;
        }

        // GET contratos/AdminContratos
        [HttpGet]
        public IActionResult GetAll () {
            var trabajadores = context_rh.Trabajador.ToList ();
            var adminCont = context.AdminContratos.ToList ();
            var adminContratos = new List<Trabajador> ();

            foreach (var item in adminCont) {
                adminContratos.Add (trabajadores.FirstOrDefault (s => s.Id == item.AdminContratoId));
            }
            return Ok (adminContratos);
        }

        // GET: contratos/AdminContratos/Id
        [HttpGet ("{id}", Name = "GetAdminContrato")]
        public IActionResult GetbyId (int id) {
            var adminContrato = context.AdminContratos.FirstOrDefault (s => s.Id == id);

            if (adminContrato == null) {
                return NotFound ();
            }
            return Ok (adminContrato);
        }

        // POST contratos/AdminContratos
        [HttpPost]
        public IActionResult POST ([FromBody] List<int> AdminContratos) {
            if (ModelState.IsValid) {
                foreach (var item in AdminContratos) {
                    var admin = context.AdminContratos.FirstOrDefault (s => s.AdminContratoId == item);
                    if (admin == null) {
                        var adminContrato = new AdminContrato {
                        AdminContratoId = item
                        };
                        context.AdminContratos.Add (adminContrato);
                        context.SaveChanges ();
                    }
                }
                return Ok ();
            }
            return BadRequest (ModelState);
        }

        // PUT contratos/adminContrato/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] AdminContrato adminContrato, int id) {
            if (adminContrato.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (adminContrato).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratos/adminContrato/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var adminContrato = context.AdminContratos.FirstOrDefault (s => s.AdminContratoId == id);

            if (adminContrato == null) {
                return NotFound ();
            }
            context.AdminContratos.Remove (adminContrato);
            context.SaveChanges ();
            return Ok (adminContrato);
        }
        // GET contratos/AdminContratos
        [HttpGet ("/contratacion/contratos/Trabajadores")]
        public IActionResult GetTrabajadores () {
            var trabajadores = context_rh.Trabajador.ToList ();
            var adminCont = context.AdminContratos.ToList ();
            var adminContratos = new List<Trabajador> ();
            var a = new AdminContrato ();

           for (int i = 0; i < adminCont.Count(); i++)
           {
               for (int j = 0; j < adminCont.Count(); j++)
               {
                  
               }
           }
            return Ok (trabajadores);
        }
    }
}