using System;
using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Dtos;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class AdminContratosController : Controller {
        private readonly ContratacionDbContext context;
        private readonly RhWebApiDbContext context_rh;

        public AdminContratosController (ContratacionDbContext context, RhWebApiDbContext context_rh) {
            this.context = context;
            this.context_rh = context_rh;
        }

        // GET contratos/AdminContratos
        [HttpGet]
        public IActionResult GetAll () {
            var trabajadores = context_rh.Trabajador.ToList ();

            var adminContratos = context.AdminContratos.Include (d => d.Departamento)
                .Select (d => new {
                    Administrador = trabajadores.FirstOrDefault (t => t.Id == d.AdminContratoId),
                        Departamento = d.Departamento,
                }).ToList ();

            return Ok (adminContratos);
        }

        // GET: contratos/AdminContratos/Id
        [HttpGet ("{id}", Name = "GetAdminContrato")]
        public IActionResult GetbyId (int id) {
         var trabajadores = context_rh.Trabajador.Select (t => new {
                Id = t.Id,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    Nombre_Completo = t.Nombre + " " + t.Apellidos,
                    CI = t.CI,
                    Sexo = t.Sexo,
                    SexoName = t.Sexo.ToString (),
                    TelefonoFijo = t.TelefonoFijo,
                    TelefonoMovil = t.TelefonoMovil,
                    Direccion = t.Direccion,
                    NivelDeEscolaridad = t.NivelDeEscolaridad,
                    NivelDeEscolaridadName = t.NivelDeEscolaridad.ToString (), MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                    PerfilOcupacional = t.PerfilOcupacional.Nombre,
                    Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                    UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                    EstadoTrabajador = t.EstadoTrabajador,
                    EstadoTrabajadorName = t.EstadoTrabajador.ToString (), Correo = t.Correo,
                    ColorDePiel = t.CaracteristicasTrab.ColorDePiel,
                    ColorDePielName = t.CaracteristicasTrab.ColorDePiel.ToString (),
                    ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos,
                    ColorDeOjosName = t.CaracteristicasTrab.ColorDeOjos.ToString (),
                    TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
                    TallaCalzado = t.CaracteristicasTrab.TallaCalzado,
                    TallaCalzadoName = t.CaracteristicasTrab.TallaCalzado.ToString (),
                    TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa,
                    TallaDeCamisaName = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
                    OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
                    Resumen = t.CaracteristicasTrab.Resumen,
                    Foto = t.CaracteristicasTrab.Foto,
                    Edad = (DateTime.Now - t.Fecha_Nac).Days / 365,
                    Departamento = ""
            });
            var adminContrato = trabajadores.FirstOrDefault (t => t.Id == id);
            if (adminContrato == null) {
                return NotFound ();
            }
            return Ok (adminContrato);
        }

        // POST contratos/AdminContratos
        [HttpPost]
        public IActionResult POST ([FromBody] AdminContratosDto adminContratosDto) {
            if (ModelState.IsValid) {
                foreach (var item in adminContratosDto.Administradores) {
                    if (context.AdminContratos.FirstOrDefault (s => s.AdminContratoId == item) == null) {
                        var administrador = new AdminContrato {
                        AdminContratoId = item,
                        DepartamentoId = adminContratosDto.DepartamentoId
                        };
                        context.AdminContratos.Add (administrador);
                        context.SaveChanges ();
                    }
                }
                return Ok ();
            }
            return BadRequest (ModelState);
        }

        // PUT contratos/adminContrato/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] AdminContratosDto adminContratosDto, int id) {
            var administrador = context.AdminContratos.FirstOrDefault (s => s.AdminContratoId == id);
            foreach (var item in adminContratosDto.Administradores) {
                administrador.AdminContratoId = item;
            }
            administrador.DepartamentoId = adminContratosDto.DepartamentoId;
            context.Entry (administrador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratos/adminContrato/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var adminContrato = context.AdminContratos.FirstOrDefault (s => s.AdminContratoId == id);

            if (adminContrato == null) {
                return NotFound ();
            }
            context.AdminContratos.Remove (adminContrato);
            context.SaveChanges ();
            return Ok (adminContrato);
        }
    }
}