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
        public ActionResult GetAll () {
            var contratos = context.Contratos.Select (t => new {
                Id = t.Id,
                    Nombre = t.Nombre,
                    Tipo = t.Tipo,
                    AdminContratoId = t.AdminContratoId,
                    EntidadId = t.EntidadId,
                    Entidad = t.Entidad.Nombre,
                    ObjetoDeContrato = t.ObjetoDeContrato,
                    Numero = t.Numero,
                    MontoCup = t.MontoCup,
                    MontoCuc = t.MontoCuc,
                    FechaDeLlegada = t.FechaDeLlegada.ToString ("dd/MM/yyyy"),
                    FechaDeVencimiento = t.FechaDeVencimiento.ToString ("dd/MM/yyyy"),
                    FechaDeFirmado = t.FechaDeFirmado.ToString ("dd/MM/yyyy"),
                    TerminoDePago = t.TerminoDePago / 30 + " Meses y " + t.TerminoDePago % 30 + " Días",
                    FormaDePago = t.FormasDePago.ToList (),
            });
            return Ok (contratos);
        }

        // GET: contratos/Contratos/Id
        [HttpGet ("{id}", Name = "GetContrato")]
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

                foreach (var item in contratoDto.FormasDePago) {
                    var contratoId_FormaPagoId = new ContratoId_FormaPagoId {
                        ContratoId = contrato.Id,
                        FormaDePagoId = item

                    };
                    context.ContratoId_FormaPagoId.Add (contratoId_FormaPagoId);
                    context.SaveChanges ();
                }

                //Agregar Juridico y Económico como Dictaminador del contrato 
                if (contratoDto.DictaminadoresId != null) {
                    foreach (var item in contratoDto.DictaminadoresId) {
                    var contratoId_DictaminadorId = new ContratoId_DictaminadorId {
                    ContratoId = contrato.Id,
                    DictaminadorContratoId = item
                        };
                        context.ContratoId_DictaminadorId.Add (contratoId_DictaminadorId);
                        context.SaveChanges ();
                    }
                } else {
                    return BadRequest ($"Tienen que dictaminar el contrato el económico y el jurídico");
                }

                //Agregar Especialistas externos como Dictaminador/es del contrato 
                if (contratoDto.EspExternoId != null) {
                    foreach (var item in contratoDto.EspExternoId) {
                    var espExternoId_ContratoId = new EspExternoId_ContratoId {
                    ContratoId = contrato.Id,
                    EspecialistaExternoId = item
                        };
                        context.EspExternoId_ContratoId.Add (espExternoId_ContratoId);
                        context.SaveChanges ();
                    }
                }

                var HistoricoEstadoContrato = new HistoricoEstadoContrato {
                    ContratoId = contrato.Id,
                    Estado = Estado.Circulando,
                    Fecha = DateTime.Now,
                    Usuario = contratoDto.Usuario,
                };
                context.Add (HistoricoEstadoContrato);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetContrato", new { id = contratoDto.Id });
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
}