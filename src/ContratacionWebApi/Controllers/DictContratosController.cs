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

        // GET DictaminadorContrato/DictaminadorContrato
        [HttpGet]
        public IEnumerable<DictaminadorContrato> GetAll()
        {
            return context.DictaminadoresContrato.ToList();
        }

        // GET: DictaminadorContrato/DictaminadorContrato/Id
        [HttpGet("{id}", Name = "Getdictaminador")]
        public IActionResult GetbyId(int id)
        {
            var dictaminador = context.DictaminadoresContrato.FirstOrDefault(s => s.Id == id);

            if (dictaminador == null)
            {
                return NotFound();
            }
            return Ok(dictaminador);
        }

        // POST DictaminadorContrato/DictaminadorContrato
        [HttpPost]
        public IActionResult POST([FromBody] DictaminadorContrato dictaminador)
        {
            if (ModelState.IsValid)
            {
                context.DictaminadoresContrato.Add(dictaminador);
                context.SaveChanges();
                return new CreatedAtRouteResult("Getdictaminador", new { id = dictaminador.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT DictaminadorContrato/dictaminador/id
        [HttpPut("{id}")]
        public IActionResult PUT([FromBody] DictaminadorContrato dictaminador, int id)
        {
            if (dictaminador.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(dictaminador).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE DictaminadorContrato/dictaminador/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dictaminador = context.DictaminadoresContrato.FirstOrDefault(s => s.Id == id);

            if (dictaminador.Id != id)
            {
                return NotFound();
            }
            context.DictaminadoresContrato.Remove(dictaminador);
            context.SaveChanges();
            return Ok(dictaminador);
        }
    }
}