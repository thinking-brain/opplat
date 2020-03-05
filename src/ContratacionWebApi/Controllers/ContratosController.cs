using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContratacionWebApi.Models;
using ContratacionWebApi.Data;

namespace ContratacionWebApi.Controllers
{
    [Route("contratacion/[controller]")]
    [ApiController]
    public class ContratosController : Controller
    {
        private readonly ContratacionDbContext context;
        public ContratosController(ContratacionDbContext context)
        {
            this.context = context;
        }

        // GET contratos/Contratos
        [HttpGet]
        public IEnumerable<Contrato> GetAll()
        {                            
            return context.Contratos.ToList();            
        }       
       
      // GET: contratos/Contratos/Id
        [HttpGet("{id}", Name = "GetCont")]
        public IActionResult GetbyId(int id)
        {
            var contrato = context.Contratos.FirstOrDefault(s => s.Id == id);

            if (contrato == null)
            {
                return NotFound();
            }
            return Ok(contrato);
           
        }

        // POST contratos/Contratos
       [HttpPost]
        public IActionResult POST([FromBody] Contrato contrato)
        {            
            if (ModelState.IsValid)
            {
                context.Contratos.Add(contrato);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetCont", new { id = contrato.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT contratos/contrato/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] Contrato contrato, int id)
        {
            if (contrato.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(contrato).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE contratos/contrato/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var contrato = context.Contratos.FirstOrDefault(s => s.Id == id);

            if (contrato.Id != id)
            {
                return NotFound();
            }
            context.Contratos.Remove(contrato);
            context.SaveChanges();
            return Ok(contrato);
       }
    }
}
