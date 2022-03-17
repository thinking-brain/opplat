using System;
using System.Collections.Generic;
using System.Linq;
using TallerWebApi.Data;
using TallerWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TallerWebApi.Controllers {
    [Route ("taller/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public ProveedoresController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/Proveedores
        [HttpGet]
        public ActionResult GetAll () {
            var proveedores = context.Proveedores.ToList ();
            return Ok (proveedores);
        }

        // GET: TallerWebApi/Proveedores/Id
        [HttpGet ("{id}", Name = "GetProveedor")]
        public IActionResult GetbyId (int id) {
            var proveedor = context.Proveedores.FirstOrDefault (s => s.Id == id);
            if (proveedor == null) {
                return NotFound ();
            }
            return Ok (proveedor);
        }

        // POST TallerWebApi/Proveedores
        [HttpPost]
        public IActionResult POST ([FromBody] Proveedor proveedor) {
             if (ModelState.IsValid) {            
                context.Proveedores.Add (proveedor);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetProveedor", new { id = proveedor.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Proveedores/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Proveedor proveedor, int id) {
            if (proveedor.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (proveedor).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Proveedores/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var proveedor = context.Proveedores.FirstOrDefault (s => s.Id == id);

            if (proveedor.Id != id) {
                return NotFound ();
            }
            context.Proveedores.Remove (proveedor);
            context.SaveChanges ();
            return Ok (proveedor);
        }
    }
}