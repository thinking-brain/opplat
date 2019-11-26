using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class TrabajadoresController : Controller {
        private readonly RhWebApiDbContext context;
        public TrabajadoresController (RhWebApiDbContext context) {
            this.context = context;
        }
        // GET recursos_humanos/trabajadores
        [HttpGet]
        public IActionResult GetAll () {
            var trabajadores = context.Trabajador.Where (t => t.EstadoTrabajador == "Activo")
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
                        MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        CargoId = t.PuestoDeTrabajo.CargoId,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                        Nombre_Completo = t.Nombre + " " + t.Apellidos,
                });

            if (trabajadores == null) {
                return NotFound ();
            }
            return Ok (trabajadores);
        }

        // GET: recursos_humanos/trabajadores/Id
        [HttpGet ("{id}", Name = "GetTrab")]
        public IActionResult GetbyId (int id) {

            var trabajador = context.Trabajador.Where (s => s.Id == id)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
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

        // POST recursos_humanos/trabajadores
        [HttpPost]
        public IActionResult POST ([FromBody] TrabajadorDto trabajadorDto) {
            if (ModelState.IsValid) {
                if (context.Trabajador.Any (e => e.CI == trabajadorDto.CI)) {
                    return BadRequest ($"El trabajador con CI {trabajadorDto.CI} ya esta en el sistema");
                }
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
                    EstadoTrabajador = "pendiente",
                };
                context.Trabajador.Add (trabajador);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTrab", new { id = trabajador.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/trabajadores/id
        [HttpPut ("{id}")]
        public async Task<IActionResult> PUT (TrabajadorDto trabajadorDto, int id) {

            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            if (id != trabajadorDto.Id) {
                return BadRequest ();
            } else if (TrabajadorExists (id)) {
                var trab = await context.Trabajador.FindAsync (id);
                trab.Nombre = trabajadorDto.Nombre;
                trab.Apellidos = trabajadorDto.Apellidos;
                trab.CI = trabajadorDto.CI;
                trab.Sexo = trabajadorDto.Sexo;
                trab.Direccion = trabajadorDto.Direccion;
                trab.MunicipioId = trabajadorDto.MunicipioId;
                trab.NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad;
                trab.TelefonoMovil = trabajadorDto.TelefonoMovil;
                trab.TelefonoFijo = trabajadorDto.TelefonoFijo;
                context.Entry (trab).State = EntityState.Modified;
                context.SaveChanges ();
                return Ok ();
            }
            try {
                await context.SaveChangesAsync ();
            } catch (DbUpdateConcurrencyException) {
                if (!TrabajadorExists (id)) {
                    return NotFound ();
                } else {
                    throw;
                }
            }
            return NoContent ();
        }

        // DELETE recursos_humanos/trabajadores/id
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

        // GET: recursos_humanos/trabajadores/Estado/estado
        [HttpGet ("/recursos_humanos/Trabajadores/Estado/{estado}")]
        public IActionResult GetTrab (string estado) {
            var trabajadores = context.Trabajador.Where (m => m.EstadoTrabajador == estado)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
                        MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                });

            if (trabajadores == null) {
                return NotFound ();
            }
            return Ok (trabajadores);
        }

        // GET: recursos_humanos/trabajadores/Sexo/sexo
        [HttpGet ("/recursos_humanos/Trabajadores/Sexo/{sexo}")]
        public IActionResult GetBySex (Sexo sexo) {
            var trabajadores = context.Trabajador
                .Where (t => t.Sexo == sexo && t.EstadoTrabajador != "pendiente")
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
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
        // GET: recursos_humanos/trabajadores/Filtro
        [HttpGet ("/recursos_humanos/Trabajadores/Filtro")]
        public IActionResult GetByFiltro (FiltroDto filtroDto) {
            var trabajadores = context.Trabajador.Select (t => new {
                Id = t.Id,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    CI = t.CI,
                    Sexo = t.Sexo.ToString (),
                    TelefonoFijo = t.TelefonoFijo,
                    TelefonoMovil = t.TelefonoMovil,
                    Direccion = t.Direccion,
                    NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
                    MunicipioId = t.MunicipioId,
                    MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                    CargoId = t.PuestoDeTrabajo.CargoId,
                    Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                    UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                    EstadoTrabajador = t.EstadoTrabajador,
                    Nombre_Completo = t.Nombre + " " + t.Apellidos,
            });
            if (!filtroDto.UnidadOrganizativa.ToString ().Equals ("")) {
                trabajadores = trabajadores.Where (t => t.UnidadOrganizativa.ToString ().Equals (filtroDto.UnidadOrganizativa.ToString ()));
            }
            if (!filtroDto.Cargo.ToString ().Equals ("")) {
                trabajadores = trabajadores.Where (t => t.Cargo.ToString ().Equals (filtroDto.Cargo.ToString ()));
            }
            if (!filtroDto.Sexo.ToString ().Equals ("")) {
                trabajadores = trabajadores.Where (t => t.Sexo.ToString ().Equals (filtroDto.Sexo.ToString ()));
            }
            if (!filtroDto.Estado.ToString ().Equals ("")) {
                trabajadores = trabajadores.Where (t => t.EstadoTrabajador.ToString ().Equals (filtroDto.Estado.ToString ()));
            }
            // if (!filtroDto.ColorDePiel.ToString ().Equals ("")) {
            //     trabajadores = trabajadores.Include (c => c.CaracteristicasTrab).Where (t => t.CaracteristicasTrab.ColorDePiel.ToString ().Equals (filtroDto.ColorDePiel.ToString ()));
            // }
            if (trabajadores == null) {
                return NotFound ();
            }
            return Ok (trabajadores);
        }
        private bool TrabajadorExists (int id) {
            return context.Trabajador.Any (e => e.Id == id);
        }
    }
}