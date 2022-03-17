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
    [Route("recursos_humanos/[controller]")]
    [ApiController]
    public class HistoricoPuestoDeTrabajoController : ControllerBase
    {
        private readonly RhWebApiDbContext context;
        public HistoricoPuestoDeTrabajoController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET recursos_humanos/HistoricoPuestoDeTrabajo
        [HttpGet]
        public IEnumerable<HistoricoPuestoDeTrabajo> GetAll()
        {                            
            return context.HistoricoPuestoDeTrabajo.ToList();            
        }       
       
      // GET: recursos_humanos/historicoPuestoDeTrabajo/Id
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

        // POST recursos_humanos/historicoPuestoDeTrabajo
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

        // PUT recursos_humanos/historicoPuestoDeTrabajo/id
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

        // DELETE recursos_humanos/historicoPuestoDeTrabajo/id
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
