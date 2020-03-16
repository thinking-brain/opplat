using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class AperturaSociosController : Controller {
        private readonly RhWebApiDbContext context;
        public AperturaSociosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/AperturaSocio
        [HttpGet]
        public IActionResult GetAll () {
            var aperturaSocios = context.AperturaSocio
                .Select (s => new {
                    Id = s.Id,
                        Fecha = s.Fecha.ToString ("dd MMMM yyyy"),
                        CantTrabajadores = s.CantTrabajadores,
                        NumeroAcuerdo = s.NumeroAcuerdo,
                        Municipio = s.CaracteristicasSocio.Municipio.Nombre,
                        Sexo = s.CaracteristicasSocio.Sexo.ToString (),
                        PerfilOcupacional = s.CaracteristicasSocio.PerfilOcupacional.Nombre,
                        NivelDeEscolaridad = s.CaracteristicasSocio.NivelDeEscolaridad.ToString (),
                        Estado = s.EstadosApertura.ToString ()
                });
            if (aperturaSocios == null) {
                return NotFound ();
            }
            return Ok (aperturaSocios);
        }

        // GET: recursos_humanos/AperturaSocio/Id
        [HttpGet ("{id}", Name = "GetApertura")]
        public IActionResult GetbyId (int id) {
            var aperturaSocio = context.AperturaSocio.Where (s => s.Id == id)
                .Select (s => new {
                    Id = s.Id,
                        Fecha = s.Fecha.ToString ("dd MMMM yyyy"),
                        CantTrabajadores = s.CantTrabajadores,
                        NumeroAcuerdo = s.NumeroAcuerdo,
                        Municipio = s.CaracteristicasSocio.Municipio.Nombre,
                        Sexo = s.CaracteristicasSocio.Sexo.ToString (),
                        PerfilOcupacional = s.CaracteristicasSocio.PerfilOcupacional.Nombre,
                        NivelDeEscolaridad = s.CaracteristicasSocio.NivelDeEscolaridad.ToString (),
                        Estado = s.EstadosApertura.ToString ()
                }).ToList ();
            if (aperturaSocio == null) {
                return NotFound ();
            }
            return Ok (aperturaSocio);
        }

        // POST recursos_humanos/AperturaSocio
        [HttpPost]
        public IActionResult POST ([FromBody] AperturaSocioDto aperturaSocioDto) {
            if (ModelState.IsValid) {
                var caratSocios = new CaracteristicasSocio () {
                    Direccion = aperturaSocioDto.CaracteristicasSocio.Direccion,
                    Sexo = aperturaSocioDto.CaracteristicasSocio.Sexo,
                    MunicipioId = aperturaSocioDto.CaracteristicasSocio.MunicipioId,
                    NivelDeEscolaridad = aperturaSocioDto.CaracteristicasSocio.NivelDeEscolaridad,
                    EdadDesde = aperturaSocioDto.CaracteristicasSocio.EdadDesde,
                    EdadHasta = aperturaSocioDto.CaracteristicasSocio.EdadHasta,
                    PerfilOcupacional = aperturaSocioDto.CaracteristicasSocio.PerfilOcupacional
                };
                context.CaracteristicasSocio.Add (caratSocios);
                context.SaveChanges ();
                var apertura = new AperturaSocio () {
                    Id = aperturaSocioDto.Id,
                    Fecha = aperturaSocioDto.Fecha,
                    CantTrabajadores = aperturaSocioDto.CantTrabajadores,
                    NumeroAcuerdo = aperturaSocioDto.NumeroAcuerdo,
                    CaracteristicasSocioId = caratSocios.Id,
                    EstadosApertura = EstadosApertura.Abierta
                };
                context.AperturaSocio.Add (apertura);
                context.SaveChanges ();

                foreach (var trabajadorId in aperturaSocioDto.ListaTrab) {
                    var trab = context.Trabajador.FirstOrDefault (t => t.Id == trabajadorId);
                    if (trab == null) {
                        return BadRequest ($"No hay trabajador con este ID");
                    }
                    trab.AperturaSocioId = apertura.Id;
                };
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetApertura", new { id = apertura.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/areas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] AperturaSocioDto aperturaSocioDto, int id) {
            if (aperturaSocioDto.Id != id) {
                return BadRequest (ModelState);
            }
            var apertura = context.AperturaSocio.Find (id);
            if (apertura != null) {
                apertura.Id = aperturaSocioDto.Id;
                apertura.Fecha = aperturaSocioDto.Fecha;
                apertura.CantTrabajadores = aperturaSocioDto.CantTrabajadores;
                apertura.NumeroAcuerdo = aperturaSocioDto.NumeroAcuerdo;
                apertura.CaracteristicasSocioId = aperturaSocioDto.CaracteristicasSocioId;
                apertura.EstadosApertura = EstadosApertura.Sin_Definir;

                foreach (var trabajadorId in aperturaSocioDto.ListaTrab) {
                    var trab = context.Trabajador.FirstOrDefault (t => t.Id == trabajadorId);
                    if (trab == null) {
                        return BadRequest ($"No hay trabajador con este ID");
                    }
                    trab.AperturaSocioId = aperturaSocioDto.Id;
                };
                context.Update (aperturaSocioDto).State = EntityState.Modified;
                context.SaveChanges ();
                return Ok ();
            }
            return NotFound ();
        }

        // DELETE recursos_humanos/AperturaSocio/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var aperturaSocio = context.AperturaSocio.FirstOrDefault (s => s.Id == id);

            if (aperturaSocio.Id != id) {
                return NotFound ();
            }
            context.AperturaSocio.Remove (aperturaSocio);
            context.SaveChanges ();
            return Ok (aperturaSocio);
        }
        // DELETE recursos_humanos/AperturaSocio/CerrarApertura/id
        [HttpGet ("/recursos_humanos/AperturaSocio/CerrarApertura/id")]
        public IActionResult CerrarApertura (int id) {
            var aperturaSocio = context.AperturaSocio.FirstOrDefault (s => s.Id == id);

            if (aperturaSocio.Id != id) {
                return NotFound ();
            }
            aperturaSocio.EstadosApertura = EstadosApertura.Cerrada;
            context.Update (aperturaSocio);
            context.SaveChanges ();
            return Ok (aperturaSocio);
        }
    }
}