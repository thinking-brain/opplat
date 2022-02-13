using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContratacionWebApi.Data;
using ContratacionWebApi.Dtos;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class ContratosController : Controller {
        private IHostingEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly ContratacionDbContext context;
        private readonly RhWebApiDbContext context_rh;
        public ContratosController (ContratacionDbContext context, IHostingEnvironment environment, RhWebApiDbContext context_rh, IEmailSender sender) {

            this.context = context;
            this.context_rh = context_rh;
            _hostingEnvironment = environment;
            _emailSender = sender;
        }

        // GET contratacion/Contratos/tipoTramite(Oferta o contrato)
        [HttpGet]
        public ActionResult GetAll (string tipoTramite, string filtro, bool cliente, string username, string roles, EstadoOrden estadoJuridico, EstadoOrden estadoEconomico, EstadoOrden estadoComiteCont, int contratoId, bool sinFechaVencimiento) {
            var trabajadores = context_rh.Trabajador.ToList ();
            var contratoId_DepartamentoId = context.ContratoId_DepartamentoId.Include (d => d.Departamento).ToList ();
            var espExternoId_ContratoId = context.EspExternoId_ContratoId.Include (d => d.EspecialistaExterno).ToList ();
            DateTime FechaPorDefecto = new DateTime (0001, 01, 01);
            var contratos = context.Contratos.Where (c => c.EstadoContrato != EstadoOrden.Cancelado).Include (c => c.Entidad).Include (c => c.Montos).Include (c => c.Dictamenes).Include (c => c.Contratos).Select (c => new {
                Id = c.Id,
                    Nombre = c.Nombre,
                    Tipo = c.Tipo,
                    TipoNombre = c.Tipo.ToString (),
                    AdminContrato = trabajadores.FirstOrDefault (t => t.Id == c.AdminContratoId),
                    Departamentos = contratoId_DepartamentoId.Where (d => d.ContratoId == c.Id).Select (d => new {
                        Id = d.Departamento.Id,
                            Nombre = d.Departamento.Nombre
                    }),
                    ObjetoDeContrato = c.ObjetoDeContrato,
                    Numero = c.Numero,
                    Montos = c.Montos.Select (d => new {
                        ContratoId = d.ContratoId,
                            Cantidad = d.Cantidad,
                            Moneda = d.Moneda,
                            NombreString = d.Moneda.ToString ()
                    }),
                    FechaDeRecepcion = c.FechaDeRecepcion,
                    FechaDeVenOferta = c.FechaDeVenOferta,
                    FechaVenContrato = c.FechaVenContrato,
                    FechaDeFirmado = c.FechaDeFirmado,
                    FechaDeRece = c.FechaDeRecepcion.ToString ("dd/MM/yyyy"),
                    FechaDeVenOfer = c.FechaDeVenOferta.ToString ("dd/MM/yyyy"),
                    FechaVenCont = c.FechaVenContrato.ToString ("dd/MM/yyyy"),
                    TerminoDePago = c.TerminoDePago,
                    TerminoDePagoDet = c.TerminoDePago + " días áviles",
                    EstadoOrden = c.EstadoContrato,
                    EstadoNombre = c.EstadoContrato.ToString (),
                    EstadoJuridico = c.EstadoJuridico,
                    EstadoJuridicoNombre = c.EstadoJuridico.ToString (),
                    EstadoEconomico = c.EstadoEconomico,
                    EstadoEconomicoNombre = c.EstadoEconomico.ToString (),
                    Cliente = c.Cliente,
                    EstadoComitContratacion = c.EstadoComitContratacion,
                    EstadoComitContratacionNombre = c.EstadoComitContratacion.ToString (),
                    OfertVence = (c.FechaDeVenOferta - DateTime.Now).Days,
                    ContVence = (c.FechaVenContrato - DateTime.Now).Days,
                    FilePath = c.FilePath,
                    MotivoSuplemento = c.MotivoSuplemento,
                    FormasDePago = context.ContratoId_FormaPagoId.Where (t => t.ContratoId == c.Id).Select (f => new {
                        Id = f.FormaDePago,
                            nombre = f.FormaDePago.ToString ()
                    }),
                    EspecialistasExternos = espExternoId_ContratoId.Where (s => s.ContratoId == c.Id).Select (e => new {
                        Id = e.EspecialistaExterno.Id,
                            NombreCompleto = e.EspecialistaExterno.NombreCompleto
                    }),
                    Dictamenes = c.Dictamenes.Select (d => new {
                        Id = d.Id,
                            Observaciones = d.Observaciones,
                            Recomendaciones = d.Recomendaciones,
                            Consideraciones = d.Consideraciones,
                            FundamentosDeDerecho = d.FundamentosDeDerecho,
                            ContratoId = d.ContratoId,
                            Contrato = d.Contrato,
                            FilePath = d.FilePath,
                            OtrosSi = d.OtrosSi,
                            FechaDictamen = d.FechaDictamen,
                            Fecha = d.FechaDictamen.ToString ("dd/MM/yyyy"),
                            Dictaminador = trabajadores.FirstOrDefault (t => t.Username == d.Username),
                            Username = d.Username
                    }),
                    Entidad = context.Entidades.Include (e => e.CuentasBancarias).Include (e => e.Telefonos)
                    .Where (s => s.Id == c.EntidadId)
                    .Select (e => new {
                        Id = e.Id,
                            Nombre = e.Nombre,
                            Siglas = e.Siglas,
                            Codigo = e.Codigo,
                            Direccion = e.Direccion,
                            Nit = e.Nit,
                            Fax = e.Fax,
                            Sector = e.Sector,
                            SectorNombre = e.Sector.ToString (),
                            Correo = e.Correo,
                            ObjetoSocial = e.ObjetoSocial,
                            Telefonos = e.Telefonos,
                            CantTelefonos = e.Telefonos.Count (),
                            CuentasBancarias = e.CuentasBancarias.Select (b => new {
                                NumeroCuenta = b.NumeroCuenta,
                                    NumeroSucursal = b.NumeroSucursal,
                                    NombreSucursal = b.NombreSucursal,
                                    NombreSucursalString = b.NombreSucursal.ToString (),
                                    Moneda = b.Moneda,
                                    MonedaString = b.Moneda.ToString ()
                            }),
                            CantCuentasBancarias = e.CuentasBancarias.Count ()
                    }),
                    ContratoId = c.ContratoId
            });
            if (tipoTramite == "oferta") {
                contratos = contratos.Where (c => c.EstadoOrden != EstadoOrden.Aprobado);
                var tiempoVenOfertas = context.TiempoVenOfertas.ToList ();
                if (tiempoVenOfertas.Count () > 0) {
                    var tiempoVenOferta = tiempoVenOfertas[0];
                    if (filtro != null) {
                        if (filtro == "ofertaTiempo") {
                            contratos = contratos.Where (c => c.OfertVence > tiempoVenOferta.OfertaTiempo);
                        }
                        if (filtro == "ofertasProxVencer") {
                            contratos = contratos.Where (c => c.OfertVence > tiempoVenOferta.OfertasProxVencDesde && c.OfertVence <= tiempoVenOferta.OfertasProxVencHasta);
                        }
                        if (filtro == "ofertasCasiVenc") {
                            contratos = contratos.Where (c => c.OfertVence >= tiempoVenOferta.OfertasCasiVencDesde && c.OfertVence <= tiempoVenOferta.OfertasCasiVencHasta);
                        }
                        if (filtro == "ofertasVenc") {
                            contratos = contratos.Where (c => c.OfertVence < tiempoVenOferta.OfertasVencidas);
                        }
                    }
                }
            }
            if (tipoTramite == "contrato") {
                var tiempoVenContratos = context.TiempoVenContratos.ToList ();
                var tiempoVenContrato = tiempoVenContratos[0];
                contratos = contratos.Where (c => c.FechaDeFirmado != FechaPorDefecto && c.EstadoOrden == EstadoOrden.Aprobado);
                if (filtro == "contratoTiempo") {
                    contratos = contratos.Where (c => c.ContVence > tiempoVenContrato.ContratoTiempo);
                }
                if (filtro == "contratosProxVencer") {
                    contratos = contratos.Where (c => c.ContVence >= tiempoVenContrato.ContratosProxVencerDesde && c.ContVence <= tiempoVenContrato.ContratosProxVencerHasta);
                }
                if (filtro == "contratosCasiVenc") {
                    contratos = contratos.Where (c => c.ContVence >= tiempoVenContrato.ContratosCasiVencDesde && c.ContVence <= tiempoVenContrato.ContratosCasiVencHasta);
                }
                if (filtro == "contratosVenc") {
                    contratos = contratos.Where (c => c.ContVence < tiempoVenContrato.ContratosVencidos);
                }
            }
            if (sinFechaVencimiento) {
                var tiempoVenContratos = context.TiempoVenContratos.ToList ();
                var tiempoVenOfertas = context.TiempoVenOfertas.ToList ();
                var tiempoVenContrato = tiempoVenContratos[0];
                var tiempoVenOferta = tiempoVenOfertas[0];
                contratos = contratos.Where (c => c.FechaVenContrato == FechaPorDefecto && c.EstadoOrden == EstadoOrden.Aprobado);
                if (filtro == "contratoTiempo") {
                    contratos = contratos.Where (c => c.ContVence > tiempoVenContrato.ContratoTiempo || c.OfertVence > tiempoVenOferta.OfertaTiempo);
                }
                if (filtro == "contratosProxVencer") {
                    contratos = contratos.Where (c => c.ContVence >= tiempoVenContrato.ContratosProxVencerDesde && c.ContVence <= tiempoVenContrato.ContratosProxVencerHasta || c.OfertVence > tiempoVenOferta.OfertasProxVencDesde && c.OfertVence <= tiempoVenOferta.OfertasProxVencHasta);
                }
                if (filtro == "contratosCasiVenc") {
                    contratos = contratos.Where (c => c.ContVence >= tiempoVenContrato.ContratosCasiVencDesde && c.ContVence <= tiempoVenContrato.ContratosCasiVencHasta || c.OfertVence >= tiempoVenOferta.OfertasCasiVencDesde && c.OfertVence <= tiempoVenOferta.OfertasCasiVencHasta);
                }
                if (filtro == "contratosVenc") {
                    contratos = contratos.Where (c => c.ContVence < tiempoVenContrato.ContratosVencidos || c.OfertVence < tiempoVenOferta.OfertasVencidas);
                }
            } else {
                if (roles != null) {
                    var data = roles.Split (",");
                    if (data.Contains (("administrador contratacion")) || data.Contains (("administrador")) ||
                        data.Contains (("juridico")) || data.Contains (("economico")) ||
                        data.Contains (("secretario comite de contratacion"))) {
                        username = null;
                    }
                }
                if (username != null) {
                    var trab = trabajadores.FirstOrDefault (t => t.Username == username);
                    contratos = contratos.Where (c => c.AdminContrato.Id == trab.Id);
                }
                if (cliente == true) {
                    contratos = contratos.Where (c => c.Cliente == true);
                } else {
                    contratos = contratos.Where (c => c.Cliente == false);
                }
                if (estadoEconomico != EstadoOrden.SinEstado || estadoJuridico != EstadoOrden.SinEstado || estadoComiteCont != EstadoOrden.SinEstado) {
                    contratos = contratos.Where (c => c.EstadoEconomico == estadoEconomico && c.EstadoJuridico == estadoJuridico && c.EstadoComitContratacion == estadoComiteCont);
                }
                if (contratoId != 0) {
                    contratos = contratos.Where (c => c.ContratoId == contratoId);
                }
            }
            return Ok (contratos);
        }

        // GET: contratacion/Contratos/Id
        [HttpGet ("{id}", Name = "GetContrato")]
        public IActionResult GetbyId (int id) {
            var contrato = context.Contratos.FirstOrDefault (c => c.Id == id);
            if (contrato == null) {
                return NotFound ();
            }
            return Ok (contrato);

        }

        // POST contratacion/Contratos
        [HttpPost]
        public async Task<IActionResult> POST (ContratoDto contratoDto) {
            if (ModelState.IsValid) {
                var contrato = new Contrato {
                    Id = contratoDto.Id,
                    Nombre = contratoDto.Nombre,
                    Tipo = contratoDto.Tipo,
                    AdminContratoId = contratoDto.AdminContrato,
                    EntidadId = contratoDto.Entidad,
                    ObjetoDeContrato = contratoDto.ObjetoDeContrato,
                    Numero = contratoDto.Numero,
                    TerminoDePago = contratoDto.TerminoDePago,
                    Cliente = contratoDto.Cliente,
                    EstadoComitContratacion = EstadoOrden.Por_Dictaminar,
                    EstadoEconomico = EstadoOrden.Por_Dictaminar,
                    EstadoJuridico = EstadoOrden.Por_Dictaminar,
                    EstadoContrato = EstadoOrden.Nuevo,
                    MotivoSuplemento = contratoDto.MotivoSuplemento
                };
                DateTime FechaPorDefecto = new DateTime (0001, 01, 01);
                if (contratoDto.FechaDeRecepcion == FechaPorDefecto) {
                    contrato.FechaDeRecepcion = DateTime.Now;
                } else {
                    contrato.FechaDeRecepcion = contratoDto.FechaDeRecepcion;
                }
                if (contratoDto.FechaDeVenOferta != FechaPorDefecto) {
                    contrato.FechaDeVenOferta = contratoDto.FechaDeVenOferta;
                } else {
                    contrato.FechaDeVenOferta = DateTime.Now.AddDays (20);
                }
                if (contratoDto.ContratoId != null) {
                    contrato.ContratoId = contratoDto.ContratoId;
                }
                context.Contratos.Add (contrato);
                context.SaveChanges ();

                if (contratoDto.Montos != null) {
                    foreach (var item in contratoDto.Montos) {
                    var monto = new Monto {
                    Cantidad = item.Cantidad,
                    Moneda = item.Moneda,
                    ContratoId = contrato.Id
                        };
                        context.Montos.Add (monto);
                    }
                    context.SaveChanges ();
                }
                foreach (var item in contratoDto.FormasDePago) {
                    var contratoId_FormaPagoId = new ContratoId_FormaPagoId {
                        ContratoId = contrato.Id,
                        FormaDePago = item
                    };
                    context.ContratoId_FormaPagoId.Add (contratoId_FormaPagoId);
                    context.SaveChanges ();
                }
                // Subir el Fichero del Contrato
                // var uploads = Path.Combine (_hostingEnvironment.WebRootPath, "uploadContratos");
                // if (file != null) {
                //     var filePath = Path.Combine (uploads, file.FileName);
                //     using (var fileStream = new FileStream (filePath, FileMode.Create)) {
                //        await file.CopyToAsync (fileStream);
                //     }
                //     contrato.FilePath=filePath;
                // }

                // Agregar Departamentos de los Dictaminadores 
                if (contratoDto.Departamentos != null) {
                    foreach (var item in contratoDto.Departamentos) {
                    var contratoId_DepartamentoId = new ContratoId_DepartamentoId {
                    ContratoId = contrato.Id,
                    DepartamentoId = item
                        };
                        context.ContratoId_DepartamentoId.Add (contratoId_DepartamentoId);
                        context.SaveChanges ();
                    }
                }

                //Agregar Especialistas externos como Dictaminador/es del contrato 
                if (contratoDto.EspecialistasExternos != null) {
                    foreach (var item in contratoDto.EspecialistasExternos) {
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
                    EstadoOrden = EstadoOrden.Nuevo,
                    Fecha = DateTime.Now,
                    Usuario = contratoDto.UserName,
                };
                context.Add (HistoricoEstadoContrato);
                var HistoricoDocumento = new HistoricoDeDocumento {
                    ContratoId = contrato.Id,
                    Username = contratoDto.UserName,
                    Fecha = DateTime.Now,
                    Detalles = "Se creó el contrato"
                };
                context.Add (HistoricoDocumento);
                context.SaveChanges ();

                var trabajadores = context_rh.Trabajador.ToList ();
                // string correos = "";

                // var dictaminadores = context.DictaminadoresContratos.Where (d => d.Departamento.Nombre == "ECONÓMICO" || d.Departamento.Nombre == "JURÍDICO")
                //     .Select (d => new {
                //         Trabajador = trabajadores.Where (x => x.Id == d.DictaminadorId).Select (t => new {
                //             Correo = t.Correo
                //         })
                //     }).ToList ();
                // foreach (var t in dictaminadores) {
                //     foreach (var item in t.Trabajador) {
                //         correos += item.Correo + " ";
                //     }
                // }
                // await _emailSender.SendEmailAsync (correos, "Nuevo Contrato", "Se ha creado un nuevo contrato que debe Dictaminar");
                return new CreatedAtRouteResult ("GetContrato", new { id = contrato.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT contratacion/contrato/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] ContratoEditDto contrato, int id) {

            if (contrato.Id != id) {
                return BadRequest (ModelState);
            }
            var c = context.Contratos.FirstOrDefault (s => s.Id == id);
            c.Id = contrato.Id;
            c.Nombre = contrato.Nombre;
            c.Tipo = contrato.Tipo;
            c.AdminContratoId = contrato.AdminContrato;
            c.EntidadId = contrato.Entidad;
            c.ObjetoDeContrato = contrato.ObjetoDeContrato;
            c.Numero = contrato.Numero;
            c.TerminoDePago = contrato.TerminoDePago;
            c.MotivoSuplemento = contrato.MotivoSuplemento;
            c.EstadoContrato = contrato.EstadoOrden;
            if (contrato.FechaDeRecepcion != null) {
                c.FechaDeRecepcion = contrato.FechaDeRecepcion;
            } else {
                c.FechaDeRecepcion = DateTime.Now;
            }
            if (contrato.FechaDeVenOferta != null) {
                c.FechaDeVenOferta = contrato.FechaDeVenOferta;
            } else {
                c.FechaDeVenOferta = DateTime.Now.AddDays (20);
            }
            if (contrato.ContratoId != null) {
                c.ContratoId = contrato.ContratoId;
            }
            context.Entry (c).State = EntityState.Modified;

            //   Agregar monto por monedas
            if (contrato.Montos != null) {
                var montos = context.Montos.Where (s => s.ContratoId == contrato.Id);
                foreach (var item in montos) {
                    context.Montos.Remove (item);
                }
                foreach (var item in contrato.Montos) {
                    var monto = new Monto {
                        Cantidad = item.Cantidad,
                        Moneda = item.Moneda,
                        ContratoId = contrato.Id
                    };
                    context.Montos.Add (monto);
                    context.SaveChanges ();
                }
            }

            if (contrato.FormasDePago != null) {
                var formasDePago = context.ContratoId_FormaPagoId.Where (s => s.ContratoId == contrato.Id);
                foreach (var item in formasDePago) {
                    context.ContratoId_FormaPagoId.Remove (item);
                }
            }
            foreach (var item in contrato.FormasDePago) {
                var contratoId_FormaPagoId = new ContratoId_FormaPagoId {
                    ContratoId = contrato.Id,
                    FormaDePago = item
                };
                context.ContratoId_FormaPagoId.Add (contratoId_FormaPagoId);
                context.SaveChanges ();
            }

            // Agregar Departamentos del contrato 
            if (contrato.Departamentos != null) {
                var departamentos = context.ContratoId_DepartamentoId.Where (s => s.ContratoId == contrato.Id);
                foreach (var item in departamentos) {
                    context.ContratoId_DepartamentoId.Remove (item);
                }
                foreach (var item in contrato.Departamentos) {
                    var contratoId_DepartamentoId = new ContratoId_DepartamentoId {
                        ContratoId = contrato.Id,
                        DepartamentoId = item
                    };
                    context.ContratoId_DepartamentoId.Add (contratoId_DepartamentoId);
                    context.SaveChanges ();
                }
            } else {
                return BadRequest ($"Tienen que Dictaminar el contrato el económico y el jurídico");
            }

            //Agregar Especialistas externos como Dictaminador/es del contrato 
            if (contrato.EspecialistasExternos != null) {
                var espExternos = context.EspExternoId_ContratoId.Where (s => s.ContratoId == contrato.Id);
                foreach (var item in espExternos) {
                    context.EspExternoId_ContratoId.Remove (item);
                }
                foreach (var item in contrato.EspecialistasExternos) {
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
                EstadoOrden = contrato.EstadoOrden,
                Fecha = DateTime.Now,
                Usuario = contrato.Username,
            };
            context.Add (HistoricoEstadoContrato);
            var HistoricoDocumento = new HistoricoDeDocumento {
                ContratoId = contrato.Id,
                Username = contrato.Username,
                Fecha = DateTime.Now,
                Detalles = "El documento ha sido editado"
            };
            context.Add (HistoricoDocumento);
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/contrato/cancelar/id
        [HttpPut ("cancelar/{id}")]
        public IActionResult Cancelar (int id) {
            var contrato = context.Contratos.FirstOrDefault (s => s.Id == id);
            if (contrato == null) {
                return NotFound ();
            }
            contrato.EstadoContrato = EstadoOrden.Cancelado;
            context.SaveChanges ();
            return Ok (contrato);
        }
        // DELETE contratacion/contrato/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var contrato = context.Contratos.FirstOrDefault (s => s.Id == id);
            if (contrato == null) {
                return NotFound ();
            }
            context.Contratos.Remove (contrato);
            context.SaveChanges ();
            return Ok (contrato);
        }
        // PUT contratacion/contratos/editNoAdminDto
        [HttpPut ("editNoAdminDto/{id}")]
        public async Task<IActionResult> DictaminarDto (DictaminarDto contrato, int id) {
            DateTime FechaPorDefecto = new DateTime (0001, 01, 01);
            var c = context.Contratos.Find (id);
            var text = "";
            if (c != null) {
                var HistoricoEstadoContrato = new HistoricoEstadoContrato {
                ContratoId = id,
                Fecha = DateTime.Now,
                Usuario = contrato.UserName
                };
                if (contrato.roles.Contains ("economico")) {
                    c.EstadoEconomico = contrato.EstadoOrden;
                    c.EstadoContrato = EstadoOrden.Circulando;
                    HistoricoEstadoContrato.EstadoOrden = contrato.EstadoOrden;
                    text = "El económico dictaminó la oferta";
                } else if (contrato.roles.Contains ("juridico")) {
                    c.Numero = contrato.Numero;
                    c.EstadoJuridico = contrato.EstadoOrden;
                    if (contrato.FechaDeFirmado != FechaPorDefecto) {
                        c.FechaDeFirmado = contrato.FechaDeFirmado;
                    }
                    c.FechaVenContrato = contrato.FechaDeVencimiento;
                    c.EstadoContrato = EstadoOrden.Circulando;
                    HistoricoEstadoContrato.EstadoOrden = contrato.EstadoOrden;
                    text = "El jurídico dictaminó la oferta";
                } else if (contrato.roles.Contains ("secretario comite de contratacion")) {
                    if (c.EstadoEconomico == EstadoOrden.No_Aprobado && c.EstadoJuridico == EstadoOrden.No_Aprobado && contrato.EstadoOrden == EstadoOrden.Aprobado) {
                        return BadRequest ($"No está aprobado por el jurídico y el económico");
                    }
                    c.EstadoComitContratacion = contrato.EstadoOrden;
                    c.EstadoContrato = contrato.EstadoOrden;
                    if (c.Tipo == Tipo.Suplemento && contrato.EstadoOrden == EstadoOrden.Aprobado) {
                        c.FechaVenContrato = contrato.FechaDeVencimiento;
                    }
                    HistoricoEstadoContrato.EstadoOrden = contrato.EstadoOrden;
                    text = "El secretario del comité de contratación dictaminó la oferta";
                } else {
                    return BadRequest ($"Los roles de este usuario no tienen permiso para aprobar la oferta");
                }
                context.Add (HistoricoEstadoContrato);

                var HistoricoDocumento = new HistoricoDeDocumento {
                    ContratoId = c.Id,
                    Username = contrato.UserName,
                    Fecha = DateTime.Now,
                    Detalles = text
                };
                context.Add (HistoricoDocumento);
                context.Update (c);
                context.SaveChanges ();

                // var admin = context_rh.Trabajador.Find (c.AdminContratoId);
                // if (admin.Correo != null) {
                //     await _emailSender.SendEmailAsync (admin.Correo, "Se ha editado el Contrato", text);
                // }
                return Ok ();
            }
            return BadRequest ("No exite dicho contrato");
        }

        // PUT contratacion/contratos/editJuridico
        [HttpPut ("editJuridico/{id}")]
        public async Task<IActionResult> EditJuridico (EditJuridico contrato, int id) {
            var c = context.Contratos.Find (id);
            if (c != null) {
                var HistoricoEstadoContrato = new HistoricoEstadoContrato {
                ContratoId = id,
                Fecha = DateTime.Now,
                Usuario = contrato.UserName
                };
                c.Numero = contrato.Numero;
                c.FechaVenContrato = contrato.FechaDeVencimiento;
                c.FechaDeFirmado = contrato.FechaDeFirmado;
                var contratoPadre = context.Contratos.Find (contrato.ContratoId);
                contratoPadre.FechaVenContrato = contrato.FechaDeVencimiento;

                context.Add (HistoricoEstadoContrato);
                context.Update (c);
                var HistoricoDocumento = new HistoricoDeDocumento {
                    ContratoId = c.Id,
                    Username = contrato.UserName,
                    Fecha = DateTime.Now,
                    Detalles = "El juridico a editado el documento, le puso una fecha de vencimiento " + contrato.FechaDeVencimiento + "y una fecha de firmado " + contrato.FechaDeFirmado
                };
                context.Add (HistoricoDocumento);
                context.Update (contratoPadre);
                context.SaveChanges ();

                // var admin = context_rh.Trabajador.Find (c.AdminContratoId);
                // if (admin.Correo != null) {
                //     await _emailSender.SendEmailAsync (admin.Correo, "Se ha editado el Contrato", $"Se le puso fecha de vencimiento al contrato {0} ",contrato.FechaDeVencimiento);
                // }
                return Ok ();
            }
            return BadRequest ("No exite dicho contrato");
        }

        // GET: contratacion/contratos/Tipos
        [HttpGet ("Tipos")]
        public IActionResult GetAllTiposContratos () {
            var tipo = new List<dynamic> () {
                new { Id = Tipo.Marco, Nombre = Tipo.Marco.ToString () },
                new { Id = Tipo.Compra, Nombre = Tipo.Compra.ToString () },
                new { Id = Tipo.Venta, Nombre = Tipo.Venta.ToString () },
                new { Id = Tipo.Agencia, Nombre = Tipo.Agencia.ToString () },
                new { Id = Tipo.Arrendamiento, Nombre = Tipo.Arrendamiento.ToString () },
                new { Id = Tipo.Comisión, Nombre = Tipo.Comisión.ToString () },
                new { Id = Tipo.Consignación, Nombre = Tipo.Consignación.ToString () },
                new { Id = Tipo.Deposito, Nombre = Tipo.Deposito.ToString () },
                new { Id = Tipo.Donación, Nombre = Tipo.Donación.ToString () },
                new { Id = Tipo.Prestación_de_Servicio, Nombre = "Prestación de Servicio" },
                new { Id = Tipo.Suministro, Nombre = Tipo.Suministro.ToString () },
                new { Id = Tipo.Transporte, Nombre = Tipo.Transporte.ToString () },
                new { Id = Tipo.Suplemento, Nombre = Tipo.Suplemento.ToString () },
            };
            return Ok (tipo);
        }
        // GET: contratacion/contratos/Estados
        [HttpGet ("Estados")]
        public IActionResult GetAllEstadosContratos () {
            var estadosContratos = new List<dynamic> () {
                new { Id = EstadoOrden.Nuevo, Nombre = EstadoOrden.Nuevo.ToString () },
                new { Id = EstadoOrden.Circulando, Nombre = EstadoOrden.Circulando.ToString () },
                new { Id = EstadoOrden.Aprobado, Nombre = "Aprobado" },
                new { Id = EstadoOrden.No_Aprobado, Nombre = "No Aprobado" },
                new { Id = EstadoOrden.Vigente, Nombre = EstadoOrden.Vigente.ToString () },
                new { Id = EstadoOrden.Cancelado, Nombre = EstadoOrden.Cancelado.ToString () },
                new { Id = EstadoOrden.Vencido, Nombre = EstadoOrden.Vencido.ToString () },
                new { Id = EstadoOrden.Por_Dictaminar, Nombre = "Por Dictaminar" },
                new { Id = EstadoOrden.SinEstado, Nombre = "Sin EstadoOrden" },
            };
            return Ok (estadosContratos);
        }
        // GET: contratacion/contratos/EstadosParaAprobar
        [HttpGet ("EstadosParaAprobar")]
        public IActionResult GetEstadosContratos () {
            var estadosContratos = new List<dynamic> () {
                new { Id = EstadoOrden.Aprobado, Nombre = "Aprobado" },
                new { Id = EstadoOrden.No_Aprobado, Nombre = "No Aprobado" },
                new { Id = EstadoOrden.Por_Dictaminar, Nombre = "Por Dictaminar" },
                new { Id = EstadoOrden.SinEstado, Nombre = " " },
            };
            return Ok (estadosContratos);
        }
        // GET: contratacion/contratos/EstadosParaAprobar
        [HttpGet ("EstadoByRool")]
        public IActionResult GetEstadoByRool (int id, string roles) {
            var c = context.Contratos.FirstOrDefault (x => x.Id == id);
            var estado = EstadoOrden.SinEstado;
            var data = roles.Split (",");
            if (data.Contains ("secretario comite de contratacion")) {
                estado = c.EstadoComitContratacion;
            } else if (data.Contains ("juridico")) {
                estado = c.EstadoJuridico;
            } else if (data.Contains ("economico")) {
                estado = c.EstadoEconomico;
            }
            return Ok (estado);
        }

        // GET: contratacion/contratos/VencimientoContrato 
        [HttpGet ("VencimientoContrato")]
        public IActionResult GetVencimientoContrato (bool cliente, string username, string roles) {
            DateTime FechaPorDefecto = new DateTime (0001, 01, 01);
            List<int> cantSegunFecha = new List<int> ();
            var tiempoVenContratos = context.TiempoVenContratos.ToList ();
            var tiempoVenContrato = tiempoVenContratos[0];
            var contratos = context.Contratos
                .Where (c => c.FechaDeFirmado != FechaPorDefecto &&
                    c.EstadoComitContratacion == EstadoOrden.Aprobado && c.EstadoContrato == EstadoOrden.Aprobado);
            if (roles != null) {
                var data = roles.Split (",");
                if (data.Contains (("administrador contratacion")) || data.Contains (("administrador")) ||
                    data.Contains (("juridico")) || data.Contains (("economico"))) {
                    username = null;
                }
            }
            if (username != null) {
                var trab = context_rh.Trabajador.FirstOrDefault (t => t.Username == username);
                contratos = contratos.Where (c => c.AdminContratoId == trab.Id);
            }
            if (cliente == true) {
                contratos = contratos.Where (c => c.Cliente == true);
            } else {
                contratos = contratos.Where (c => c.Cliente == false);
            }
            // Contratos vencidos           
            cantSegunFecha.Add (contratos
                .Where (c => (c.FechaVenContrato - DateTime.Now).Days < tiempoVenContrato.ContratosVencidos).Count ());

            // Contratos Casi Vencidas
            cantSegunFecha.Add (contratos
                .Where (c => (c.FechaVenContrato - DateTime.Now).Days >= tiempoVenContrato.ContratosCasiVencDesde && (c.FechaVenContrato - DateTime.Now).Days <= tiempoVenContrato.ContratosCasiVencHasta).Count ());

            // Contratos Próximos a Vencer
            cantSegunFecha.Add (contratos
                .Where (c => (c.FechaVenContrato - DateTime.Now).Days >= tiempoVenContrato.ContratosProxVencerDesde && (c.FechaVenContrato - DateTime.Now).Days <= tiempoVenContrato.ContratosProxVencerHasta).Count ());

            // Contratos OK
            cantSegunFecha.Add (contratos.Where (c => (c.FechaVenContrato - DateTime.Now).Days > tiempoVenContrato.ContratoTiempo).Count ());

            return Ok (cantSegunFecha);
        }

        // GET: contratacion/contratos/VencimientoOferta 
        [HttpGet ("VencimientoOferta")]
        public IActionResult GetVencimientoOferta (bool cliente, string username, string roles) {
            List<int> cantSegunFecha = new List<int> ();
            var tiempoVenOfertas = context.TiempoVenOfertas.ToList ();
            var tiempoVenOferta = tiempoVenOfertas[0];
            var ofertas = context.Contratos
                .Where (c => c.EstadoContrato != EstadoOrden.Aprobado && c.EstadoContrato != EstadoOrden.Cancelado);

            if (roles != null) {
                var data = roles.Split (",");
                if (data.Contains (("administrador contratacion")) || data.Contains (("administrador")) ||
                    data.Contains (("juridico")) || data.Contains (("economico")) ||
                    data.Contains (("secretario comite de contratacion"))) {
                    username = null;
                }
            }
            if (username != null) {
                var trab = context_rh.Trabajador.FirstOrDefault (t => t.Username == username);
                ofertas = ofertas.Where (c => c.AdminContratoId == trab.Id);
            }
            if (cliente == true) {
                ofertas = ofertas.Where (c => c.Cliente == true);
            } else {
                ofertas = ofertas.Where (c => c.Cliente == false);
            }
            // Ofertas vencidos           
            cantSegunFecha.Add (ofertas
                .Where (c => (c.FechaDeVenOferta - DateTime.Now).Days < tiempoVenOferta.OfertasVencidas).Count ());

            // Ofertas Casi Vencidas
            cantSegunFecha.Add (ofertas
                .Where (c => (c.FechaDeVenOferta - DateTime.Now).Days >= tiempoVenOferta.OfertasCasiVencDesde && (c.FechaDeVenOferta - DateTime.Now).Days <= tiempoVenOferta.OfertasCasiVencHasta).Count ());

            // Ofertas Próximos a Vencer
            cantSegunFecha.Add (ofertas
                .Where (c => (c.FechaDeVenOferta - DateTime.Now).Days > tiempoVenOferta.OfertasProxVencDesde && (c.FechaDeVenOferta - DateTime.Now).Days <= tiempoVenOferta.OfertasProxVencHasta).Count ());

            // Ofertas OK
            cantSegunFecha.Add (ofertas.Where (c => (c.FechaDeVenOferta - DateTime.Now).Days > tiempoVenOferta.OfertaTiempo).Count ());

            return Ok (cantSegunFecha);
        }

        //Post :contratacion/contratos/UploadFile
        [HttpPost ("/contratacion/contratos/UploadFile/{id}")]
        public async Task<IActionResult> UploadFile (IFormFile file, int id) {
            var contrato = context.Contratos.FirstOrDefault (s => s.Id == id);
            var adminContrato = context.AdminContratos.FirstOrDefault (c => c.AdminContratoId == contrato.AdminContratoId);
            var departamento = context.Departamentos.FirstOrDefault (d => d.Id == adminContrato.DepartamentoId);
            if (contrato == null) {
                return NotFound ($"El Contrato no se encuentra");
            }
            if (file != null) {
                string folderName = Path.Combine (_hostingEnvironment.WebRootPath, "Contratos");
                string subFolder = System.IO.Path.Combine (folderName, departamento.Nombre);
                var name = file.FileName.Split (".");
                string subFolder1 = System.IO.Path.Combine (subFolder, name[0]);

                if (!Directory.Exists (subFolder1)) {
                    System.IO.Directory.CreateDirectory (subFolder1);
                }
                var filePath = Path.Combine (subFolder1, file.FileName);
                using (var fileStream = new FileStream (filePath, FileMode.Create)) {
                    await file.CopyToAsync (fileStream);
                }
                contrato.FilePath = filePath;
                context.SaveChanges ();
                return Ok ();
            } else {
                return NotFound ($"El archivo es null");
            }
        }

        private Dictionary<string, string> GetMimeTypes () {
            return new Dictionary<string, string> { { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }

        //Get :contratacion/contratos/downloadFile
        [HttpGet ("/contratacion/contratos/DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile (int id) {
            var contrato = context.Contratos.FirstOrDefault (c => c.Id == id);
            if (contrato.FilePath != null) {
                var path = contrato.FilePath;
                var memory = new MemoryStream ();
                using (var stream = new FileStream (path, FileMode.Open)) { await stream.CopyToAsync (memory); }
                memory.Position = 0;
                var ext = Path.GetExtension (path).ToLowerInvariant ();
                return File (memory, GetMimeTypes () [ext], Path.GetFileName (path));
            }
            return NotFound ($"No tiene un documento guardado");
        }

        [HttpGet ("Dashboard")]
        public async Task<IActionResult> Dashboard (string username, string roles) {
            var dashboard = new Dashboard ();
            DateTime FechaPorDefecto = new DateTime (0001, 01, 01);
            var cantOfertas_x_Mes = new int[12];
            var cantOfertVen_x_Mes = new int[12];
            var contratosProximosVencer = new int[5];
            var tiempoVenOfertas = context.TiempoVenOfertas.ToList ();
            var tiempoVenOferta = tiempoVenOfertas[0];

            var ofertasContratos = context.Contratos.Where (c => c.FechaDeRecepcion.Year == DateTime.Now.Year);
            var contratos = context.Contratos.Where (c => c.FechaDeFirmado != FechaPorDefecto &&
                c.EstadoComitContratacion == EstadoOrden.Aprobado && c.EstadoContrato == EstadoOrden.Aprobado);

            var ofertas = context.Contratos.Where (c => c.FechaDeFirmado == FechaPorDefecto && c.EstadoContrato != EstadoOrden.Aprobado);

            if (roles != null) {
                var data = roles.Split (",");
                if (data.Contains (("administrador contratacion")) || data.Contains (("administrador")) ||
                    data.Contains (("juridico")) || data.Contains (("economico")) ||
                    data.Contains (("secretario comite de contratacion"))) {
                    username = null;
                }
            }
            if (username != null) {
                var trab = context_rh.Trabajador.FirstOrDefault (t => t.Username == username);
                contratos = contratos.Where (c => c.AdminContratoId == trab.Id);
                ofertas = ofertas.Where (c => c.AdminContratoId == trab.Id);
                ofertasContratos = ofertasContratos.Where (c => c.AdminContratoId == trab.Id);
            }
            if (ofertas != null) {
                // Grafico Lineal Ofertas //
                for (int i = 0; i < 12; i++) {
                    cantOfertas_x_Mes[i] = ofertas.Where (c => c.EstadoContrato != EstadoOrden.Cancelado &&
                        c.EstadoContrato != EstadoOrden.No_Aprobado && c.EstadoContrato != EstadoOrden.SinEstado && c.FechaDeRecepcion.Month == i + 1).Count ();
                    cantOfertVen_x_Mes[i] = ofertas
                        .Where (c => c.FechaDeRecepcion.Month == i + 1 &&
                            (c.FechaDeVenOferta - DateTime.Now).Days < tiempoVenOferta.OfertasVencidas).Count ();
                }
                dashboard.OfertasProceso = cantOfertas_x_Mes;
                dashboard.OfertasVencidas = cantOfertVen_x_Mes;
                // Grafico Lineal Ofertas//

                // Tabla Ofertas//
                dashboard.OfertasEnProceso = ofertas.Where (c => c.EstadoContrato != EstadoOrden.Cancelado &&
                    c.EstadoContrato != EstadoOrden.No_Aprobado && c.EstadoContrato != EstadoOrden.SinEstado).Count ();

                dashboard.OfertasVenHastaFecha = ofertas.Where (c => (c.FechaDeVenOferta - DateTime.Now).Days < tiempoVenOferta.OfertasVencidas).Count ();

                dashboard.OfertasVenEsteMes = ofertas.Where (c => c.FechaDeVenOferta.Month == DateTime.Now.Month &&
                    (c.FechaDeVenOferta - DateTime.Now).Days < tiempoVenOferta.OfertasVencidas).Count ();
            }
            dashboard.OfertasProcesadas = ofertasContratos.Where (c => c.FechaDeRecepcion.Year == DateTime.Now.Year).Count ();
            if (contratos != null) {
                double TiempoCirculacion = 0;
                double TiempoCirculacionMes = 0;
                foreach (var item in contratos) {
                    TiempoCirculacion += (item.FechaDeFirmado - item.FechaDeRecepcion).Days;
                }
                foreach (var item in contratos.Where (c => c.FechaDeRecepcion.Month == DateTime.Now.Month)) {
                    TiempoCirculacionMes += (item.FechaDeFirmado - item.FechaDeRecepcion).Days;
                }
                dashboard.PromCircuOferta = Math.Round (TiempoCirculacion / contratos.Count (), 0);
                dashboard.PromCircuOfertaMes = Math.Round (TiempoCirculacionMes / contratos
                    .Where (c => c.FechaDeRecepcion.Month == DateTime.Now.Month).Count (), 0);
                // Tabla Ofertas//

                // Contratos Próximos a Vencer //
                contratosProximosVencer[0] = contratos.Where (c => c.FechaVenContrato.Month == DateTime.Now.Month).Count ();

                contratosProximosVencer[1] = contratos.Where (c => c.FechaVenContrato.Month == (DateTime.Now.Month + 1) ||
                    c.FechaVenContrato.Month == (DateTime.Now.Month + 2) || c.FechaVenContrato.Month == (DateTime.Now.Month + 3)).Count ();

                contratosProximosVencer[2] = contratos.Where (c => c.FechaVenContrato.Month == (DateTime.Now.Month + 1) ||
                    c.FechaVenContrato.Month == (DateTime.Now.Month + 2) || c.FechaVenContrato.Month == (DateTime.Now.Month + 3) ||
                    c.FechaVenContrato.Month == (DateTime.Now.Month + 4) || c.FechaVenContrato.Month == (DateTime.Now.Month + 5) ||
                    c.FechaVenContrato.Month == (DateTime.Now.Month + 6)).Count ();

                contratosProximosVencer[3] = contratos.Where (c => c.FechaVenContrato.Year == DateTime.Now.Year).Count ();
                contratosProximosVencer[4] = contratos.Where (c => c.FechaVenContrato.Year == DateTime.Now.Year + 1).Count ();
                dashboard.ContratosProximosVencer = contratosProximosVencer;
                // Contratos Próximos a Vencer //
            }

            return Ok (dashboard);
        }
    }
}