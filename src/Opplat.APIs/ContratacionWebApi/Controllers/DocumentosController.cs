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
    public class DocumentosController : ControllerBase
    {
        private readonly ContratacionDbContext context;
        public DocumentosController(ContratacionDbContext context)
        {
            this.context = context;
        }

        // GET contratacion/Documentos
        [HttpGet]
        public IEnumerable<Documento> GetAll()
        {
            return context.Documentos.ToList();
        }

        // GET: contratacion/Documentos/Id
        [HttpGet("{id}", Name = "GetDocumento")]
        public IActionResult GetbyId(int id)
        {
            var documento = context.Documentos.FirstOrDefault(s => s.Id == id);

            if (documento == null)
            {
                return NotFound();
            }
            return Ok(documento);
        }

        // POST contratacion/Documentos
        [HttpPost]
        public IActionResult POST([FromBody] Documento documento)
        {
            if (ModelState.IsValid)
            {
                context.Documentos.Add(documento);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetDocumento", new { id = documento.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT contratacion/Documentos/id
        [HttpPut("{id}")]
        public IActionResult PUT([FromBody] Documento documento, int id)
        {
            if (documento.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(documento).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE contratacion/Documentos/id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var documento = context.Documentos.FirstOrDefault(s => s.Id == id);

            if (documento.Id != id)
            {
                return NotFound();
            }
            context.Documentos.Remove(documento);
            context.SaveChanges();
            return Ok(documento);
        }
    }
}