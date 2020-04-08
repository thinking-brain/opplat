﻿using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class FormasDePagosController : Controller {
        private readonly ContratacionDbContext context;
        public FormasDePagosController (ContratacionDbContext context) {
            this.context = context;
        }
        // GET contratacion/FormasDePagos
        [HttpGet]
        public IActionResult GetAll () {
            var formasDePagos = new List<dynamic> () {
                new { Id = FormaDePago.Transferencia, Nombre = FormaDePago.Transferencia.ToString () },
                new { Id = FormaDePago.Cheque, Nombre = FormaDePago.Cheque.ToString () },
                new { Id = FormaDePago.Efectivo, Nombre = FormaDePago.Efectivo.ToString () },
            };
            return Ok (formasDePagos);
        }

        //     // GET contratacion/FormasDePagos
        //     [HttpGet]
        //     public IEnumerable<FormaDePago> GetAll()
        //     {
        //         return context.FormasDePagos.ToList();
        //     }

        //     // GET: contratacion/FormasDePagos/Id
        //     [HttpGet("{id}", Name = "GetformaDePago")]
        //     public IActionResult GetbyId(int id)
        //     {
        //         var formaDePago = context.FormasDePagos.FirstOrDefault(s => s.Id == id);

        //         if (formaDePago == null)
        //         {
        //             return NotFound();
        //         }
        //         return Ok(formaDePago);
        //     }

        //     // POST contratacion/FormasDePagos
        //     [HttpPost]
        //     public IActionResult POST([FromBody] FormaDePago formaDePago)
        //     {
        //         if (ModelState.IsValid)
        //         {
        //             context.FormasDePagos.Add(formaDePago);
        //             context.SaveChanges();
        //             return new CreatedAtRouteResult("GetformaDePago", new { id = formaDePago.Id });
        //         }
        //         return BadRequest(ModelState);
        //     }

        //     // PUT contratacion/formaDePago/id
        //     [HttpPut("{id}")]
        //     public IActionResult PUT([FromBody] FormaDePago formaDePago, int id)
        //     {
        //         if (formaDePago.Id != id)
        //         {
        //             return BadRequest(ModelState);

        //         }
        //         context.Entry(formaDePago).State = EntityState.Modified;
        //         context.SaveChanges();
        //         return Ok();
        //     }

        //     // DELETE contratacion/formaDePago/id
        //     [HttpDelete("{id}")]
        //     public IActionResult Delete(int id)
        //     {
        //         var formaDePago = context.FormasDePagos.FirstOrDefault(s => s.Id == id);

        //         if (formaDePago.Id != id)
        //         {
        //             return NotFound();
        //         }
        //         context.FormasDePagos.Remove(formaDePago);
        //         context.SaveChanges();
        //         return Ok(formaDePago);
        //     }
    }
}