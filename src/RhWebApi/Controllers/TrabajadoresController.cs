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
            var trabajadores = context.Trabajador.Where (s => s.EstadoTrabajador != Estados.Baja s.Bolsa == false)
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
                        //MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        //CargoId = t.PuestoDeTrabajo.CargoId,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador.ToString (),
                        Correo = t.Correo,
                        Perfil_Ocupacional = t.Perfil_Ocupacional,
                        Nombre_Completo = t.Nombre + " " + t.Apellidos,
                        ColorDePiel = t.CaracteristicasTrab.ColorDePiel.ToString (),
                        ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos.ToString (),
                        TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
                        TallaCalzado = t.CaracteristicasTrab.TallaCalzado.ToString (),
                        TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
                        OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
                        Resumen = t.CaracteristicasTrab.Resumen,
                        Foto = t.CaracteristicasTrab.Foto,
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
                        //  MunicipioId = t.MunicipioId,
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        // CargoId = t.PuestoDeTrabajo.CargoId,
                        Perfil_Ocupacional = t.Perfil_Ocupacional,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador.ToString (),
                        Correo = t.Correo,
                        Nombre_Completo = t.Nombre + " " + t.Apellidos,
                        ColorDePiel = t.CaracteristicasTrab.ColorDePiel.ToString (),
                        ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos.ToString (),
                        TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
                        TallaCalzado = t.CaracteristicasTrab.TallaCalzado.ToString (),
                        TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
                        OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
                        Resumen = t.CaracteristicasTrab.Resumen,
                        Foto = t.CaracteristicasTrab.Foto,
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
                    return BadRequest ($"El trabajador ya está en el sistema");
                } else {
                    var trabajador = new Trabajador () {
                        Nombre = trabajadorDto.Nombre,
                        Apellidos = trabajadorDto.Apellidos,
                        CI = trabajadorDto.CI,
                        Sexo = trabajadorDto.Sexo,
                        Direccion = trabajadorDto.Direccion,
                        NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad,
                        TelefonoMovil = trabajadorDto.TelefonoMovil,
                        TelefonoFijo = trabajadorDto.TelefonoFijo,
                        EstadoTrabajador = Estados.Bolsa,
                        Perfil_Ocupacional = trabajadorDto.Perfil_Ocupacional,

                    };
                    context.Trabajador.Add (trabajador);
                    context.SaveChanges ();
                    var caracteristicas = new CaracteristicasTrab () {
                        TrabajadorId = trabajador.Id,
                        ColorDeOjos = trabajadorDto.ColorDeOjos,
                        ColorDePiel = trabajadorDto.ColorDePiel,
                        TallaDeCamisa = trabajadorDto.TallaDeCamisa,
                        TallaPantalon = trabajadorDto.TallaPantalon,
                        TallaCalzado = trabajadorDto.TallaCalzado,
                        OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas,
                    };

                    context.CaracteristicasTrab.Add (caracteristicas);
                    context.SaveChanges ();

                    var trabBolsa = new Bolsa () {
                        TrabajadorId = trabajador.Id,
                        Fecha = DateTime.Now,
                        Nombre_Referencia = trabajadorDto.Nombre_Referencia,
                    };
                    context.Bolsa.Add (trabBolsa);
                    context.SaveChanges ();
                    return new CreatedAtRouteResult ("GetTrab", new { id = trabajador.Id });
                }
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/trabajadores/id
        [HttpPut ("{id}")]
        public async Task<IActionResult> PUT ([FromBody] TrabajadorDto trabajadorDto, int id) {

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
                trab.Perfil_Ocupacional = trabajadorDto.Perfil_Ocupacional;
                trab.NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad;
                trab.TelefonoMovil = trabajadorDto.TelefonoMovil;
                trab.TelefonoFijo = trabajadorDto.TelefonoFijo;
                trab.CaracteristicasTrab.ColorDeOjos = trabajadorDto.ColorDeOjos;
                trab.CaracteristicasTrab.ColorDePiel = trabajadorDto.ColorDePiel;
                trab.CaracteristicasTrab.TallaDeCamisa = trabajadorDto.TallaDeCamisa;
                trab.CaracteristicasTrab.TallaPantalon = trabajadorDto.TallaPantalon;
                trab.CaracteristicasTrab.TallaCalzado = trabajadorDto.TallaCalzado;
                trab.CaracteristicasTrab.OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas;
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

            if (!TrabajadorExists (id)) {
                return NotFound ();
            }
            context.Trabajador.Remove (trabajador);
            context.SaveChanges ();
            return Ok (trabajador);
        }

        // GET: recursos_humanos/trabajadores/Filtro
        [HttpGet ("/recursos_humanos/Trabajadores/Filtro")]
        public IActionResult GetByFiltro (string UnidadOrganizativa = "", string Cargo = "", string Sexo = "", string Estado = "", string ColorDePiel = "", string NivelDeEscolaridad = "") {
            var trabajadores = context.Trabajador.Select (t => new {
                Id = t.Id,
                    Nombre = t.Nombre,
                    Apellidos = t.Apellidos,
                    Nombre_Completo = t.Nombre + " " + t.Apellidos,
                    CI = t.CI,
                    Sexo = t.Sexo.ToString (),
                    TelefonoFijo = t.TelefonoFijo,
                    TelefonoMovil = t.TelefonoMovil,
                    Direccion = t.Direccion,
                    NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
                    //MunicipioId = t.MunicipioId,
                    MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                    //CargoId = t.PuestoDeTrabajo.CargoId,
                    Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                    UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                    EstadoTrabajador = t.EstadoTrabajador.ToString (),
                    Correo = t.Correo,
                    ColorDePiel = t.CaracteristicasTrab.ColorDePiel.ToString (),
                    ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos.ToString (),
                    TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
                    TallaCalzado = t.CaracteristicasTrab.TallaCalzado.ToString (),
                    TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
                    OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
                    Resumen = t.CaracteristicasTrab.Resumen,
                    Foto = t.CaracteristicasTrab.Foto,
            });
            if (!string.IsNullOrEmpty (UnidadOrganizativa)) {
                trabajadores = trabajadores.Where (t => t.UnidadOrganizativa.ToString ().Equals (UnidadOrganizativa));
            }
            if (!string.IsNullOrEmpty (Cargo)) {
                trabajadores = trabajadores.Where (t => t.Cargo.ToString ().Equals (Cargo));
            }
            if (!string.IsNullOrEmpty (Sexo)) {
                trabajadores = trabajadores.Where (t => t.Sexo.ToString ().Equals (Sexo));
            }
            if (!string.IsNullOrEmpty (Estado)) {
                trabajadores = trabajadores.Where (t => t.EstadoTrabajador.ToString ().Equals (Estado));
            }
            // if (!string.IsNullOrEmpty (ColorDePiel)) {
            //     trabajadores = trabajadores.Where (t => t.ColorDePiel.Equals (ColorDePiel.ToString ()));
            // }
            if (!string.IsNullOrEmpty (NivelDeEscolaridad)) {
                trabajadores = trabajadores.Where (t => t.NivelDeEscolaridad.Equals (NivelDeEscolaridad.ToString ()));
            }
            if (trabajadores == null) {
                return NotFound ();
            } else {
                return Ok (trabajadores);
            }
        }
        // GET: recursos_humanos/trabajadores/Bolsa
        [HttpGet ("/recursos_humanos/Trabajadores/Bolsa")]
        public IActionResult GetAllBolsa () {
            var trab = context.Bolsa.Include (b => b.Trabajador)
                .Select (t => new {
                    Id = t.Trabajador.Id,
                        Nombre = t.Trabajador.Nombre,
                        Apellidos = t.Trabajador.Apellidos,
                        Nombre_Completo = t.Trabajador.Nombre + " " + t.Trabajador.Apellidos,
                        CI = t.Trabajador.CI,
                        // Sexo = t.Sexo.ToString (),
                        // TelefonoFijo = t.TelefonoFijo,
                        // TelefonoMovil = t.TelefonoMovil,
                        // Direccion = t.Direccion,
                        // NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
                        // Perfil_Ocupacional = t.Perfil_Ocupacional,
                        // MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        // Correo = t.Correo,
                        // ColorDePiel = t.CaracteristicasTrab.ColorDePiel.ToString (),
                        // ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos.ToString (),
                        // TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
                        // TallaCalzado = t.CaracteristicasTrab.TallaCalzado.ToString (),
                        // TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
                        // OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
                        // Resumen = t.CaracteristicasTrab.Resumen,
                        // Foto = t.CaracteristicasTrab.Foto,
                        recomendado = t.Nombre_Referencia
                });
            // var trabajadores = context.Trabajador.Where (s => s.Bolsa)
            //     .Select (t => new {
            //         Id = t.Id,
            //             Nombre = t.Nombre,
            //             Apellidos = t.Apellidos,
            //             Nombre_Completo = t.Nombre + " " + t.Apellidos,
            //             CI = t.CI,
            //             Sexo = t.Sexo.ToString (),
            //             TelefonoFijo = t.TelefonoFijo,
            //             TelefonoMovil = t.TelefonoMovil,
            //             Direccion = t.Direccion,
            //             NivelDeEscolaridad = t.NivelDeEscolaridad.ToString (),
            //             Perfil_Ocupacional = t.Perfil_Ocupacional,
            //             MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
            //             Correo = t.Correo,
            //             ColorDePiel = t.CaracteristicasTrab.ColorDePiel.ToString (),
            //             ColorDeOjos = t.CaracteristicasTrab.ColorDeOjos.ToString (),
            //             TallaPantalon = t.CaracteristicasTrab.TallaPantalon,
            //             TallaCalzado = t.CaracteristicasTrab.TallaCalzado.ToString (),
            //             TallaDeCamisa = t.CaracteristicasTrab.TallaDeCamisa.ToString (),
            //             OtrasCaracteristicas = t.CaracteristicasTrab.OtrasCaracteristicas,
            //             Resumen = t.CaracteristicasTrab.Resumen,
            //             Foto = t.CaracteristicasTrab.Foto,
            //     });

            if (trab == null) {
                return NotFound ();
            }
            return Ok (trab);
        }
        private bool TrabajadorExists (int id) {
            return context.Trabajador.Any (e => e.Id == id);
        }
    }
}