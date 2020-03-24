using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using RhWebApi.Data;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class ContratosController : Controller {
        private readonly ContratacionDbContext context;
        // private readonly RhWebApiDbContext context_rh;
        public ContratosController (ContratacionDbContext context) {
            this.context = context;
            // this.context_rh = context_rh;
        }

        // GET contratos/Contratos/tipoTramite(proforma o contrato)
        [HttpGet]
        public ActionResult GetAll (string tipoTramite) {
            // var trabajadores = context_rh.Trabajador.ToList ();

            var contratos = context.Contratos.Select (c => new {
                Id = c.Id,
                    Nombre = c.Nombre,
                    TipoId = c.Tipo,
                    Tipo = c.Tipo.ToString (),
                    AdminContratoId = c.AdminContratoId,
                    // AdminContrato = trabajadores.FirstOrDefault (t => t.Id == c.AdminContratoId),
                    EntidadId = c.EntidadId,
                    Entidad = c.Entidad.Nombre,
                    ObjetoDeContrato = c.ObjetoDeContrato,
                    Numero = c.Numero,
                    MontoCup = c.MontoCup,
                    MontoCuc = c.MontoCuc,
                    FechaDeLlegada = c.FechaDeLlegada.ToString ("dd/MM/yyyy"),
                    FechaDeVencimiento = c.FechaDeVencimiento,
                    FechaDeFirmado = c.FechaDeFirmado,
                    TerminoDePago = c.TerminoDePago / 30 + " Meses y " + c.TerminoDePago % 30 + " Días",
                    EstadoId = c.Estado,
                    Estado = c.Estado.ToString (),
                    AprobJuridico = c.AprobJuridico,
                    AprobEconomico = c.AprobEconomico,
                    AprobComitContratacion = c.AprobComitContratacion
            });
            if (tipoTramite == "proforma") {
                contratos = contratos.Where (c => c.FechaDeFirmado == null && c.AprobComitContratacion == false);
            }
            if (tipoTramite == "contrato") {
                contratos = contratos.Where (c => c.FechaDeFirmado == null && c.AprobComitContratacion == true);
            }
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
        // PUT contratos/contrato/AprobContrato/id
        [HttpPut ("/contratos/contrato/AprobContrato/id")]
        public IActionResult AprobContrato ([FromBody] AproContratoDto aproContratoDto, int id) {
            var c = context.Contratos.Find (id);

            if (c != null) {
                if (aproContratoDto.AprobEconomico == true) {
                    c.AprobEconomico = true;
                }
                if (aproContratoDto.AprobJuridico == true) {
                    c.AprobJuridico = true;
                }
                if (aproContratoDto.AprobComitContratacion == true) {
                    c.AprobComitContratacion = true;
                }

                context.Update (c);
                context.SaveChanges ();
                return Ok ();
            }
            return NotFound ();
        }
        // GET: contratacion/contratos/Tipos
        [HttpGet ("/contratacion/contratos/Tipos")]
        public IActionResult GetAllTiposContratos () {
            var tipo = new List<dynamic> () {
                new { Id = Tipo.Marco, Nombre = Tipo.Marco.ToString () },
                new { Id = Tipo.Compra, Nombre = Tipo.Compra.ToString () },
                new { Id = Tipo.Venta, Nombre = Tipo.Venta.ToString () },
                new { Id = Tipo.Servicio, Nombre = Tipo.Servicio.ToString () },
            };
            return Ok (tipo);
        }
        // GET: contratacion/contratos/Tipos
        [HttpGet ("/contratacion/contratos/Estados")]
        public IActionResult GetAllEstadosContratos () {
            var tipo = new List<dynamic> () {
                new { Id = Estado.Nuevo, Nombre = Estado.Nuevo.ToString () },
                new { Id = Estado.Circulando, Nombre = Estado.Circulando.ToString () },
                new { Id = Estado.Aprobado, Nombre = Estado.Aprobado.ToString () },
                new { Id = Estado.NoAprobado, Nombre = Estado.NoAprobado.ToString () },
                new { Id = Estado.Vigente, Nombre = Estado.Vigente.ToString () },
                new { Id = Estado.Cancelado, Nombre = Estado.Cancelado.ToString () },
                new { Id = Estado.Vencido, Nombre = Estado.Vencido.ToString () },
                new { Id = Estado.Revision, Nombre = Estado.Revision.ToString () },
                new { Id = Estado.SinEstado, Nombre = Estado.SinEstado.ToString () },
            };
            return Ok (tipo);
        }
    }
}