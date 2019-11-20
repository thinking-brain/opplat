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
    public class ActividadContratoController : Controller
    {
        private readonly RhWebApiDbContext context;
        public ActividadContratoController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET recursos_humanos/ActividadContrato
        [HttpGet]
        public IEnumerable<ActividadContrato> GetAll()
        {                            
            return context.ActividadContrato.ToList();            
        }       
       
      // GET: recursos_humanos/ActividadContrato/Id
        [HttpGet("{id}", Name = "GetActCont")]
        public IActionResult GetbyId(int id)
        {
            var actividadContrato = context.ActividadContrato.FirstOrDefault(s => s.Id == id);

            if (actividadContrato == null)
            {
                return NotFound();
            }
            return Ok(actividadContrato);
           
        }

        // POST recursos_humanos/actividadContrato
       [HttpPost]
        public IActionResult POST([FromBody] ActividadContrato actividadContrato)
        {            
            if (ModelState.IsValid)
            {
                context.ActividadContrato.Add(actividadContrato);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetActCont", new { id = actividadContrato.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT recursos_humanos/actividadContrato/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] ActividadContrato actividadContrato, int id)
        {
            if (actividadContrato.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(actividadContrato).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE recursos_humanos/actividadContrato/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var actividadContrato = context.ActividadContrato.FirstOrDefault(s => s.Id == id);

            if (actividadContrato.Id != id)
            {
                return NotFound();
            }
            context.ActividadContrato.Remove(actividadContrato);
            context.SaveChanges();
            return Ok(actividadContrato);
       }
    }
}
