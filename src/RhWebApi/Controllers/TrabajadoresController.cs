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
            var trabajadores = context.Trabajador.Where (s => s.EstadoTrabajador != Estados.Baja || s.EstadoTrabajador != Estados.Bolsa)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = t.Nombre,
                        Apellidos = t.Apellidos,
                        CI = t.CI,
                        Sexo = t.Sexo,
                        SexoName = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad,
                        NivelDeEscolaridadName = t.NivelDeEscolaridad.ToString (),
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador,
                        EstadoTrabajadorName = t.EstadoTrabajador.ToString (),
                        Correo = t.Correo,
                        Perfil_Ocupacional = t.Perfil_Ocupacional,
                        Nombre_Completo = t.Nombre + " " + t.Apellidos,
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
                        Correo = trabajadorDto.Correo,
                        NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad,
                        TelefonoMovil = trabajadorDto.TelefonoMovil,
                        TelefonoFijo = trabajadorDto.TelefonoFijo,
                        EstadoTrabajador = Estados.Bolsa,
                        Perfil_Ocupacional = trabajadorDto.Perfil_Ocupacional
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
                        OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas
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
        public IActionResult PUT ([FromBody] TrabajadorDto trabajadorDto, int id) {

            if (ModelState.IsValid) {
                var t = context.Trabajador.Find (id);
                if (t != null) {
                    t.Nombre = trabajadorDto.Nombre;
                    t.Apellidos = trabajadorDto.Apellidos;
                    t.CI = trabajadorDto.CI;
                    t.Sexo = trabajadorDto.Sexo;
                    t.Direccion = trabajadorDto.Direccion;
                    t.Correo = trabajadorDto.Correo;
                    t.MunicipioId = trabajadorDto.MunicipioId;
                    t.Perfil_Ocupacional = trabajadorDto.Perfil_Ocupacional;
                    t.NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad;
                    t.TelefonoMovil = trabajadorDto.TelefonoMovil;
                    t.TelefonoFijo = trabajadorDto.TelefonoFijo;
                    context.Update (t);
                    context.SaveChanges ();

                    var caractTrab = context.CaracteristicasTrab.SingleOrDefault (c => c.TrabajadorId == id);
                    if (caractTrab != null) {
                        caractTrab.Foto = trabajadorDto.Foto;
                        caractTrab.ColorDeOjos = trabajadorDto.ColorDeOjos;
                        caractTrab.ColorDePiel = trabajadorDto.ColorDePiel;
                        caractTrab.TallaDeCamisa = trabajadorDto.TallaDeCamisa;
                        caractTrab.TallaPantalon = trabajadorDto.TallaPantalon;
                        caractTrab.TallaCalzado = trabajadorDto.TallaCalzado;
                        caractTrab.OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas;
                        context.Update (caractTrab);
                        if (t.EstadoTrabajador == Estados.Bolsa) {
                            var trabBolsa = context.Bolsa.SingleOrDefault (c => c.TrabajadorId == id);
                            trabBolsa.Nombre_Referencia = trabajadorDto.Nombre_Referencia;
                            context.Update (trabBolsa);
                            context.SaveChanges ();
                        }
                        context.SaveChanges ();
                    } else {
                        var caracteristicas = new CaracteristicasTrab () {
                            TrabajadorId = trabajadorDto.Id,
                            TallaPantalon = trabajadorDto.TallaPantalon,
                            TallaCalzado = trabajadorDto.TallaCalzado,
                            ColorDeOjos = trabajadorDto.ColorDeOjos,
                            ColorDePiel = trabajadorDto.ColorDePiel,
                            TallaDeCamisa = trabajadorDto.TallaDeCamisa,
                            OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas
                        };
                        context.CaracteristicasTrab.Add (caracteristicas);
                        context.SaveChanges ();
                        if (t.EstadoTrabajador == Estados.Bolsa) {
                            var trabBolsa = new Bolsa () {
                            TrabajadorId = trabajadorDto.Id,
                            Fecha = DateTime.Now,
                            Nombre_Referencia = trabajadorDto.Nombre_Referencia,
                            };
                            context.Bolsa.Add (trabBolsa);
                            context.SaveChanges ();
                        }
                        return Ok ();
                    }
                }
                return NotFound ();
            }
            return BadRequest (ModelState);
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
            if (!string.IsNullOrEmpty (ColorDePiel)) {
                trabajadores = trabajadores.Where (t => t.ColorDePiel.Equals (ColorDePiel.ToString ()));
            }
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
                        Sexo = t.Trabajador.Sexo,
                        SexoName = t.Trabajador.Sexo.ToString (),
                        TelefonoFijo = t.Trabajador.TelefonoFijo,
                        TelefonoMovil = t.Trabajador.TelefonoMovil,
                        Direccion = t.Trabajador.Direccion,
                        NivelDeEscolaridad = t.Trabajador.NivelDeEscolaridad,
                        NivelDeEscolaridadName = t.Trabajador.NivelDeEscolaridad.ToString (),
                        Perfil_Ocupacional = t.Trabajador.Perfil_Ocupacional,
                        MunicipioProv = t.Trabajador.Municipio.Nombre + " " + t.Trabajador.Municipio.Provincia.Nombre,
                        Correo = t.Trabajador.Correo,
                        ColorDePiel = t.Trabajador.CaracteristicasTrab.ColorDePiel,
                        ColorDePielName = t.Trabajador.CaracteristicasTrab.ColorDePiel.ToString (),
                        ColorDeOjos = t.Trabajador.CaracteristicasTrab.ColorDeOjos,
                        ColorDeOjosName = t.Trabajador.CaracteristicasTrab.ColorDeOjos.ToString (),
                        TallaPantalon = t.Trabajador.CaracteristicasTrab.TallaPantalon,
                        TallaCalzado = t.Trabajador.CaracteristicasTrab.TallaCalzado,
                        TallaCalzadoName = t.Trabajador.CaracteristicasTrab.TallaCalzado.ToString (),
                        TallaDeCamisa = t.Trabajador.CaracteristicasTrab.TallaDeCamisa,
                        TallaDeCamisaName = t.Trabajador.CaracteristicasTrab.TallaDeCamisa.ToString (),
                        OtrasCaracteristicas = t.Trabajador.CaracteristicasTrab.OtrasCaracteristicas,
                        Foto = t.Trabajador.CaracteristicasTrab.Foto,
                        Referencia = t.Nombre_Referencia,
                        Fecha_Entrada = t.Fecha.ToString ("dd MMMM yyyy"),
                        Tiempo_Bolsa = (DateTime.Now - t.Fecha).Days + " Días",
                });

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