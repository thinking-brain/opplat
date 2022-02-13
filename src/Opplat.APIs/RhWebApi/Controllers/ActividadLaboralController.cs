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
    public class ActividadLaboralController : Controller
    {
        private readonly RhWebApiDbContext context;
        public ActividadLaboralController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET recursos_humanos/ActividadLaboral
        [HttpGet]
        public IEnumerable<ActividadLaboral> GetAll()
        {                            
            return context.ActividadLaboral.ToList();            
        }       
       
      // GET: recursos_humanos/ActividadLaboral/Id
        [HttpGet("{id}", Name = "GetActividadLaboral")]
        public IActionResult GetbyId(int id)
        {
            var actividadContratoTrab = context.ActividadLaboral.FirstOrDefault(s => s.Id == id);

            if (actividadContratoTrab == null)
            {
                return NotFound();
            }
            return Ok(actividadContratoTrab);
           
        }

        // POST recursos_humanos/ActividadLaboral
       [HttpPost]
        public IActionResult POST([FromBody] ActividadLaboral actividadLaboral)
        {            
            if (ModelState.IsValid)
            {
                context.ActividadLaboral.Add(actividadLaboral);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetActividadLaboral", new { id = actividadLaboral.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT recursos_humanos/actividadLaboral/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] ActividadLaboral actividadLaboral, int id)
        {
            if (actividadLaboral.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(actividadLaboral).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE recursos_humanos/actividadLaboral/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var actividadLaboral = context.ActividadLaboral.FirstOrDefault(s => s.Id == id);

            if (actividadLaboral.Id != id)
            {
                return NotFound();
            }
            context.ActividadLaboral.Remove(actividadLaboral);
            context.SaveChanges();
            return Ok(actividadLaboral);
       }
    }
}
