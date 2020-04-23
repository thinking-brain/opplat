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
    public class DictContratosController : Controller
    {
        private readonly ContratacionDbContext context;
        public DictContratosController(ContratacionDbContext context)
        {
            this.context = context;
        }

        // GET Dictaminador/Dictaminador
        [HttpGet]
        public IEnumerable<Dictaminador> GetAll()
        {
            return context.Dictaminadores.ToList();
        }

        // GET: Dictaminador/Dictaminador/Id
        [HttpGet("{id}", Name = "Getdictaminador")]
        public IActionResult GetbyId(int id)
        {
            var dictaminador = context.Dictaminadores.FirstOrDefault(s => s.Id == id);

            if (dictaminador == null)
            {
                return NotFound();
            }
            return Ok(dictaminador);
        }

        // POST Dictaminador/Dictaminador
        [HttpPost]
        public IActionResult POST([FromBody] Dictaminador dictaminador)
        {
            if (ModelState.IsValid)
            {
                context.Dictaminadores.Add(dictaminador);
                context.SaveChanges();
                return new CreatedAtRouteResult("Getdictaminador", new { id = dictaminador.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT Dictaminador/dictaminador/id
        [HttpPut("{id}")]
        public IActionResult PUT([FromBody] Dictaminador dictaminador, int id)
        {
            if (dictaminador.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(dictaminador).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE Dictaminador/dictaminador/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dictaminador = context.Dictaminadores.FirstOrDefault(s => s.Id == id);

            if (dictaminador.Id != id)
            {
                return NotFound();
            }
            context.Dictaminadores.Remove(dictaminador);
            context.SaveChanges();
            return Ok(dictaminador);
        }
    }
}