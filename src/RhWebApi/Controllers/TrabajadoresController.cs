using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrabajadoresController : Controller
    {
        private readonly RhWebApiContext context;
        public TrabajadoresController(RhWebApiContext context)
        {
            this.context = context;
        }

        // GET api/trabajadores
        [HttpGet]
        public IEnumerable<Trabajador> GetAll()
        {                
             var trabajadores = context.Trabajador.Where(s => (s.TipoDeContrato == TipoDeContratoTrabajador.Fijo || s.TipoDeContrato==TipoDeContratoTrabajador.Temporal) && s.TipoDeContrato != TipoDeContratoTrabajador.Socio && s.Estado != EstadoTrabajador.Baja);

            return trabajadores.ToList();            
        }       
       
      // GET: api/trabajadores/Id
        [HttpGet("{id}", Name = "GetTrab")]
        public IActionResult GetbyId(int id)
        {
            var trabajador = context.Trabajador.FirstOrDefault(s => s.Id == id);

            if (trabajador == null)
            {
                return NotFound();
            }
            return Ok(trabajador);
           
        }

        // POST api/trabajadores
       [HttpPost]
        public IActionResult POST([FromBody] Trabajador trabajador)
        {            
            if (ModelState.IsValid)
            {
                context.Trabajador.Add(trabajador);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetTrab", new { id = trabajador.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT api/trabajadores/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] Trabajador trabajador, int id)
        {
            if (trabajador.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(trabajador).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE api/trabajadores/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var trabajador = context.Trabajador.FirstOrDefault(s => s.Id == id);

            if (trabajador.Id != id)
            {
                return NotFound();
            }
            context.Trabajador.Remove(trabajador);
            context.SaveChanges();
            return Ok(trabajador);
       }
    }
}
