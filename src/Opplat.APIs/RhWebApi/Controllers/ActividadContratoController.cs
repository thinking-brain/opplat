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
    public class ActividadContratoTrabController : ControllerBase
    {
        private readonly RhWebApiDbContext context;
        public ActividadContratoTrabController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET recursos_humanos/ActividadContratoTrab
        [HttpGet]
        public IEnumerable<ActividadContratoTrab> GetAll()
        {                            
            return context.ActividadContratoTrab.ToList();            
        }       
       
      // GET: recursos_humanos/ActividadContratoTrab/Id
        [HttpGet("{id}", Name = "GetActCont")]
        public IActionResult GetbyId(int id)
        {
            var actividadContratoTrab = context.ActividadContratoTrab.FirstOrDefault(s => s.Id == id);

            if (actividadContratoTrab == null)
            {
                return NotFound();
            }
            return Ok(actividadContratoTrab);
           
        }

        // POST recursos_humanos/actividadContratoTrab
       [HttpPost]
        public IActionResult POST([FromBody] ActividadContratoTrab actividadContratoTrab)
        {            
            if (ModelState.IsValid)
            {
                context.ActividadContratoTrab.Add(actividadContratoTrab);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetActCont", new { id = actividadContratoTrab.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT recursos_humanos/actividadContratoTrab/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] ActividadContratoTrab actividadContratoTrab, int id)
        {
            if (actividadContratoTrab.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(actividadContratoTrab).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE recursos_humanos/actividadContratoTrab/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var actividadContratoTrab = context.ActividadContratoTrab.FirstOrDefault(s => s.Id == id);

            if (actividadContratoTrab.Id != id)
            {
                return NotFound();
            }
            context.ActividadContratoTrab.Remove(actividadContratoTrab);
            context.SaveChanges();
            return Ok(actividadContratoTrab);
       }
    }
}
