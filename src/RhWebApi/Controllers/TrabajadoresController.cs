using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class TrabajadoresController : Controller {
        private readonly RhWebApiDbContext context;
        public TrabajadoresController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET api/trabajadores
        [HttpGet]
        public IActionResult GetAll () {
            var trabajadores = context.Trabajador
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo,
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad,
                        MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        CargoId = t.PuestoDeTrabajo.CargoId,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                });

            if (trabajadores == null) {
                return NotFound ();
            }
            return Ok (trabajadores);
        }

        // GET: api/trabajadores/estado
        [HttpGet ("/api/TrabByEstado/{estado}")]
        public IActionResult GetTrab (string estado) {
            var trabajadores = context.Trabajador.Where (m => m.EstadoTrabajador == estado)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo,
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad,
                        MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        CargoId = t.PuestoDeTrabajo.CargoId,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                });

            if (trabajadores == null) {
                return NotFound ();
            }
            return Ok (trabajadores);
        }

        // GET: api/trabajadores/Id
        [HttpGet ("{id}", Name = "GetTrab")]
        public IActionResult GetbyId (int id) {

            var trabajador = context.Trabajador.Where (s => s.Id == id)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo,
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad,
                        MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        CargoId = t.PuestoDeTrabajo.CargoId,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                });

            if (trabajador == null) {
                return NotFound ();
            }
            return Ok (trabajador);
        }

        // POST api/trabajadores
        [HttpPost]
        public IActionResult POST ([FromBody] TrabajadorDto trabajadorDto) {
            if (ModelState.IsValid) {
                var trabajador = new Trabajador () {
                    Nombre = trabajadorDto.Nombre,
                    Apellidos = trabajadorDto.Apellidos,
                    CI = trabajadorDto.CI,
                    Sexo = trabajadorDto.Sexo,
                    Direccion = trabajadorDto.Direccion,
                    MunicipioId = trabajadorDto.MunicipioId,
                    NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad,
                    TelefonoMovil = trabajadorDto.TelefonoMovil,
                    TelefonoFijo = trabajadorDto.TelefonoFijo,
                };
                context.Trabajador.Add (trabajador);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTrab", new { id = trabajador.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT api/trabajadores/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] TrabajadorDto trabajadorDto, int id) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!context.Set<Trabajador>().Any(c => c.Id == id))
            {
                return NotFound($"No se encuentra el Trabajador con id {id}");
            }           
            var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == id);
          
            trabajador.Id = trabajadorDto.Id;
            trabajador.Nombre = trabajadorDto.Nombre;
            trabajador.Apellidos = trabajadorDto.Apellidos;
            trabajador.CI = trabajadorDto.CI;
            trabajador.Sexo = trabajadorDto.Sexo;
            trabajador.Direccion = trabajadorDto.Direccion;
            trabajador.NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad;
            trabajador.TelefonoFijo = trabajadorDto.TelefonoFijo;
            trabajador.TelefonoMovil = trabajadorDto.TelefonoMovil;
            trabajador.MunicipioId = trabajadorDto.MunicipioId;

            context.Entry (trabajador).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE api/trabajadores/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == id);

            if (trabajador.Id != id) {
                return NotFound ();
            }
            context.Trabajador.Remove (trabajador);
            context.SaveChanges ();
            return Ok (trabajador);
        }       
    }
}