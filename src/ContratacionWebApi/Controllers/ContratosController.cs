using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class ContratosController : Controller {
        private readonly ContratacionDbContext context;
        public ContratosController (ContratacionDbContext context) {
            this.context = context;
        }

        // GET contratos/Contratos
        [HttpGet]
        public IEnumerable<Contrato> GetAll () {
            return context.Contratos.ToList ();
        }

    // GET: contratos/Contratos/Id
    [HttpGet ("{id}", Name = "GetCont")]
    public IActionResult GetbyId (int id) {
        var contrato = context.Contratos.FirstOrDefault (s => s.Id == id);

        if (contrato == null) {
            return NotFound ();
        }
        return Ok (contrato);

    }

    // POST contratos/Contratos
    [HttpPost]
    public IActionResult POST ([FromBody] ContratoDto contratoDto) {
        if (ModelState.IsValid) {
            var contrato = new Contrato {
                Id = contratoDto.Id,
                Nombre = contratoDto.Nombre,
                Tipo = contratoDto.Tipo,
                AdminContratoId = contratoDto.AdminContratoId,
                EntidadId = contratoDto.EntidadId,
                ObjetoDeContrato = contratoDto.ObjetoDeContrato,
                Numero = contratoDto.Numero,
                MontoCup = contratoDto.MontoCup,
                MontoCuc = contratoDto.MontoCuc,
                FechaDeLlegada = contratoDto.FechaDeLlegada,
                FechaDeVencimiento = contratoDto.FechaDeVencimiento,
                FechaDeFirmado = contratoDto.FechaDeFirmado,
                TerminoDePago = contratoDto.TerminoDePago,
            };
            context.Contratos.Add (contrato);
            context.SaveChanges ();

            //estado del contrato>
            var estado = Estado.SinEstado;
            if (contrato.FechaDeFirmado != null) {
                estado = Estado.Aprobado;
            }
            //estado del contrato/>

            var HistoricoEstadoContrato = new HistoricoEstadoContrato {
                ContratoId = contrato.Id,
                Estado = estado,
                Fecha = DateTime.Now,
                Usuario = contratoDto.Usuario,
            };
            context.Add (HistoricoEstadoContrato);
            context.SaveChanges ();
            return new CreatedAtRouteResult ("GetCont", new { id = contratoDto.Id });
        }
        return BadRequest (ModelState);
    }

    // PUT contratos/contrato/id
    [HttpPut ("{id}")]
    public IActionResult PUT ([FromBody] Contrato contrato, int id) {
        if (contrato.Id != id) {
            return BadRequest (ModelState);

        }
        context.Entry (contrato).State = EntityState.Modified;
        context.SaveChanges ();
        return Ok ();
    }

    // DELETE contratos/contrato/id
    [HttpDelete ("{id}")]
    public IActionResult Delete (int id) {
        var contrato = context.Contratos.FirstOrDefault (s => s.Id == id);

        if (contrato.Id != id) {
            return NotFound ();
        }
        context.Contratos.Remove (contrato);
        context.SaveChanges ();
        return Ok (contrato);
    }
}