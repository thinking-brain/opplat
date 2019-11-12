using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;
using RhWebApi.Data;


namespace RhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricoPuestoDeTrabajoController : Controller
    {
        private readonly RhWebApiDbContext context;
        public HistoricoPuestoDeTrabajoController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET api/HistoricoPuestoDeTrabajo
        [HttpGet]
        public IEnumerable<HistoricoPuestoDeTrabajo> GetAll()
        {                            
            return context.HistoricoPuestoDeTrabajo.ToList();            
        }       
       
      // GET: api/historicoPuestoDeTrabajo/Id
        [HttpGet("{id}", Name = "GetHistoricoPuestoDeTrabajo")]
        public IActionResult GetbyId(int id)
        {
            var historicoPuestoDeTrabajo = context.HistoricoPuestoDeTrabajo.FirstOrDefault(s => s.Id == id);

            if (historicoPuestoDeTrabajo == null)
            {
                return NotFound();
            }
            return Ok(historicoPuestoDeTrabajo);
           
        }

        // POST api/historicoPuestoDeTrabajo
       [HttpPost]
        public IActionResult POST([FromBody] HistoricoPuestoDeTrabajo historicoPuestoDeTrabajo)
        {            
            if (ModelState.IsValid)
            {
                context.HistoricoPuestoDeTrabajo.Add(historicoPuestoDeTrabajo);
                context.SaveChanges();
                return new CreatedAtRouteResult("GethistoricoPuestoDeTrabajo", new { id = historicoPuestoDeTrabajo.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT api/historicoPuestoDeTrabajo/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] HistoricoPuestoDeTrabajo historicoPuestoDeTrabajo, int id)
        {
            if (historicoPuestoDeTrabajo.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(historicoPuestoDeTrabajo).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE api/historicoPuestoDeTrabajo/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var historicoPuestoDeTrabajo = context.HistoricoPuestoDeTrabajo.FirstOrDefault(s => s.Id == id);

            if (historicoPuestoDeTrabajo.Id != id)
            {
                return NotFound();
            }
            context.HistoricoPuestoDeTrabajo.Remove(historicoPuestoDeTrabajo);
            context.SaveChanges();
            return Ok(historicoPuestoDeTrabajo);
       }
    }
}
