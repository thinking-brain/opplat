using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers
{
    [Route("contratacion/[controller]")]
    [ApiController]
    public class AdminContratosController : Controller
    {
        private readonly ContratacionDbContext context;
        public AdminContratosController(ContratacionDbContext context)
        {
            this.context = context;
        }

        // GET entidades/AdminContratos
        [HttpGet]
        public IActionResult GetAll()
        {
            var adminContratos = context.AdminContratos.ToList();
            return Ok(adminContratos);
        }

        // GET: entidades/AdminContratos/Id
        [HttpGet("{id}", Name = "GetAdminContrato")]
        public IActionResult GetbyId(int id)
        {
            var adminContrato = context.AdminContratos.FirstOrDefault(s => s.Id == id);

            if (adminContrato == null)
            {
                return NotFound();
            }
            return Ok(adminContrato);
        }

        // POST entidades/AdminContratos
        [HttpPost]
        public IActionResult POST([FromBody] AdminContrato adminContrato)
        {
            if (ModelState.IsValid)
            {
                context.AdminContratos.Add(adminContrato);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetAdminContrato", new { id = adminContrato.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT entidades/adminContrato/id
        [HttpPut("{id}")]
        public IActionResult PUT([FromBody] AdminContrato adminContrato, int id)
        {
            if (adminContrato.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(adminContrato).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE entidades/adminContrato/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var adminContrato = context.AdminContratos.FirstOrDefault(s => s.Id == id);

            if (adminContrato.Id != id)
            {
                return NotFound();
            }
            context.AdminContratos.Remove(adminContrato);
            context.SaveChanges();
            return Ok(adminContrato);
        }
    }
}