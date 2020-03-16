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
    public class EspecialistasExternosController : Controller
    {
        private readonly ContratacionDbContext context;
        public EspecialistasExternosController(ContratacionDbContext context)
        {
            this.context = context;
        }

        // GET EspecialistasExternos/EspecialistasExternos
        [HttpGet]
        public IEnumerable<EspecialistaExterno> GetAll()
        {
            return context.EspecialistasExternos.ToList();
        }

        // GET: EspecialistasExternos/EspecialistasExternos/Id
        [HttpGet("{id}", Name = "GetespecialistaExterno")]
        public IActionResult GetbyId(int id)
        {
            var especialistaExterno = context.EspecialistasExternos.FirstOrDefault(s => s.Id == id);

            if (especialistaExterno == null)
            {
                return NotFound();
            }
            return Ok(especialistaExterno);
        }

        // POST EspecialistasExternos/EspecialistasExternos
        [HttpPost]
        public IActionResult POST([FromBody] EspecialistaExterno especialistaExterno)
        {
            if (ModelState.IsValid)
            {
                context.EspecialistasExternos.Add(especialistaExterno);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetespecialistaExterno", new { id = especialistaExterno.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT EspecialistasExternos/especialistaExterno/id
        [HttpPut("{id}")]
        public IActionResult PUT([FromBody] EspecialistaExterno especialistaExterno, int id)
        {
            if (especialistaExterno.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(especialistaExterno).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE EspecialistasExternos/especialistaExterno/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var especialistaExterno = context.EspecialistasExternos.FirstOrDefault(s => s.Id == id);

            if (especialistaExterno.Id != id)
            {
                return NotFound();
            }
            context.EspecialistasExternos.Remove(especialistaExterno);
            context.SaveChanges();
            return Ok(especialistaExterno);
        }
    }
}