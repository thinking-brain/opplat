using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            var trabajadores = context.Trabajador
                .Where (s =>
                    s.EstadoTrabajador != Estados.Baja &&
                    s.EstadoTrabajador != Estados.Bolsa &&
                    s.EstadoTrabajador != Estados.Descartado)
                .Select (t => new {
                    Id = t.Id,
                        Nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase (t.Nombre),
                        Apellidos = t.Apellidos,
                        Nombre_Completo = CultureInfo.InvariantCulture.TextInfo.ToTitleCase (t.Nombre + " " + t.Apellidos),
                        CI = t.CI,
                        Sexo = t.Sexo,
                        SexoName = t.Sexo.ToString (),
                        TelefonoFijo = t.TelefonoFijo,
                        TelefonoMovil = t.TelefonoMovil,
                        Direccion = t.Direccion,
                        NivelDeEscolaridad = t.NivelDeEscolaridad,
                        NivelDeEscolaridadName = t.NivelDeEscolaridad.ToString (),
                        PerfilOcupacionalId = t.PerfilOcupacional.Id,
                        PerfilOcupacional = t.PerfilOcupacional.Nombre,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        CargoId = t.PuestoDeTrabajo.Cargo.Id,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        UnidadOrganizativaId = t.PuestoDeTrabajo.UnidadOrganizativa.Id,
                        EstadoTrabajador = t.EstadoTrabajador,
                        EstadoTrabajadorName = t.EstadoTrabajador.ToString (),
                        Correo = t.Correo,
                        Municipio = t.Municipio.Nombre,
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
                        Fecha_Nac = t.Fecha_Nac.ToString ("dd MMMM yyyy"),
                        Edad = (DateTime.Now - t.Fecha_Nac).Days / 365,
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
                        MunicipioProv = t.Municipio.Nombre + " " + t.Municipio.Provincia.Nombre,
                        Municipio = t.Municipio.Nombre,
                        UnidadOrganizativa = t.PuestoDeTrabajo.UnidadOrganizativa.Nombre,
                        Cargo = t.PuestoDeTrabajo.Cargo.Nombre,
                        EstadoTrabajador = t.EstadoTrabajador.ToString (),
                        Correo = t.Correo,
                        Nombre_Completo = CultureInfo.InvariantCulture.TextInfo.ToTitleCase (t.Nombre + " " + t.Apellidos),
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
                var trab = context.Trabajador.FirstOrDefault (s => s.CI == trabajadorDto.CI);
                if (trab != null) {
                    if (trab.EstadoTrabajador == Estados.Descartado) {
                        return BadRequest ($"El trabajador ya está en el sistema y fue descartado de la bolsa");
                    }
                    return BadRequest ($"El trabajador ya está en el sistema");
                } else {
                    // Saber sexo y fecha de cumpleaños por el CI
                    var sexo = new Sexo ();
                    string fecha = "";
                    DateTime fechaNac = new DateTime ();
                    IFormatProvider culture = new System.Globalization.CultureInfo ("en-US", true);
                    if (trabajadorDto.CI != null) {
                        var sexoCI = int.Parse (trabajadorDto.CI.Substring (9, 1));
                        if (sexoCI % 2 == 0) {
                            sexo = Sexo.M;
                        } else {
                            sexo = Sexo.F;
                        }
                        var siglo = int.Parse (trabajadorDto.CI.Substring (6, 1));
                        if (siglo >= 0 && siglo <= 5) {
                            fecha = "19" + trabajadorDto.CI.Substring (0, 6);
                        }
                        if (siglo >= 6 && siglo <= 8) {
                            fecha = "20" + trabajadorDto.CI.Substring (0, 6);
                        }
                        fechaNac = DateTime.ParseExact (fecha, "yyyyMMdd", culture);
                    }
                    //Crear Trabajador
                    var trabajador = new Trabajador () {
                        Nombre = trabajadorDto.Nombre,
                        Apellidos = trabajadorDto.Apellidos,
                        CI = trabajadorDto.CI,
                        Sexo = sexo,
                        Direccion = trabajadorDto.Direccion,
                        Correo = trabajadorDto.Correo,
                        NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad,
                        TelefonoMovil = trabajadorDto.TelefonoMovil,
                        TelefonoFijo = trabajadorDto.TelefonoFijo,
                        EstadoTrabajador = Estados.Bolsa,
                        PerfilOcupacionalId = trabajadorDto.PerfilOcupacionalId,
                        Fecha_Nac = fechaNac,
                    };
                    context.Trabajador.Add (trabajador);
                    context.SaveChanges ();
                    //Crear Caracteristicas del Trabajador
                    var caracteristicas = new CaracteristicasTrab () {
                        TrabajadorId = trabajador.Id,
                        Foto = trabajadorDto.Foto,
                        ColorDeOjos = trabajadorDto.ColorDeOjos,
                        ColorDePiel = trabajadorDto.ColorDePiel,
                        TallaDeCamisa = trabajadorDto.TallaDeCamisa,
                        TallaPantalon = trabajadorDto.TallaPantalon,
                        TallaCalzado = trabajadorDto.TallaCalzado,
                        OtrasCaracteristicas = trabajadorDto.OtrasCaracteristicas
                    };

                    context.CaracteristicasTrab.Add (caracteristicas);
                    context.SaveChanges ();
                    //Agregar Datos del trabajador en la bolsa
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
            var sexo = new Sexo ();
            string fecha = "";
            DateTime fechaNac = new DateTime ();
            IFormatProvider culture = new System.Globalization.CultureInfo ("en-US", true);
            if (ModelState.IsValid) {
                var t = context.Trabajador.Find (id);
                if (t != null) {
                    t.Nombre = trabajadorDto.Nombre;
                    t.Apellidos = trabajadorDto.Apellidos;
                    t.Direccion = trabajadorDto.Direccion;
                    t.Correo = trabajadorDto.Correo;
                    t.MunicipioId = trabajadorDto.MunicipioId;
                    t.PerfilOcupacionalId = trabajadorDto.PerfilOcupacionalId;
                    t.NivelDeEscolaridad = trabajadorDto.NivelDeEscolaridad;
                    t.TelefonoMovil = trabajadorDto.TelefonoMovil;
                    t.TelefonoFijo = trabajadorDto.TelefonoFijo;

                    //Cambio del sexo y fecha Nacimiento en caso de que cambie el CI
                    if (t.CI != trabajadorDto.CI) {
                        if (trabajadorDto.CI != null) {
                            var sexoCI = int.Parse (trabajadorDto.CI.Substring (9, 1));
                            if (sexoCI % 2 == 0) {
                                sexo = Sexo.M;
                            } else {
                                sexo = Sexo.F;
                            }
                            var siglo = int.Parse (trabajadorDto.CI.Substring (6, 1));
                            if (siglo >= 0 && siglo <= 5) {
                                fecha = "19" + trabajadorDto.CI.Substring (0, 6);
                            }
                            if (siglo >= 6 && siglo <= 8) {
                                fecha = "20" + trabajadorDto.CI.Substring (0, 6);
                            }
                            t.Sexo = sexo;
                            fechaNac = DateTime.ParseExact (fecha, "yyyyMMdd", culture);
                            t.Fecha_Nac = fechaNac;
                        }
                    }
                    t.CI = trabajadorDto.CI;
                    context.Update (t);

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

                        //En Caso que esté en bolsa
                        if (t.EstadoTrabajador == Estados.Bolsa) {
                            var trabBolsa = context.Bolsa.SingleOrDefault (c => c.TrabajadorId == id);
                            if (trabBolsa != null) {
                                trabBolsa.Nombre_Referencia = trabajadorDto.Nombre_Referencia;
                                context.Update (trabBolsa);
                            } else {
                                var newTrabBolsa = new Bolsa () {
                                    TrabajadorId = trabajadorDto.Id,
                                    Fecha = DateTime.Now,
                                    Nombre_Referencia = trabajadorDto.Nombre_Referencia,
                                };
                                context.Bolsa.Add (newTrabBolsa);
                            }
                        }
                        context.SaveChanges ();
                        return Ok ();
                    }
                }
                return NotFound ();
            }
            return BadRequest (ModelState);
        }

        // Pasar a Descartado el Trabajador en Bolsa
        // DELETE recursos_humanos/trabajadores/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var trabajador = context.Trabajador.FirstOrDefault (s => s.Id == id);

            if (!TrabajadorExists (id)) {
                return NotFound ();
            }
            trabajador.EstadoTrabajador = Estados.Descartado;
            context.SaveChanges ();
            return Ok (trabajador);
        }
        // GET: recursos_humanos/trabajadores/Filtro
        [HttpGet ("/recursos_humanos/Trabajadores/Filtro")]
        public IActionResult GetByFiltro (
            bool bolsa, string UnidadOrganizativa = "",
            string Cargo = "", string Sexo = "", string Estado = "",
            string ColorDePiel = "", string NivelDeEscolaridad = "",
            string EdadDesde = "", string EdadHasta = "",
            string PerfilOcupacional = "", string Municipio = "") {
            var trabajadores = context.Trabajador.Select (t => new {
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
                    Edad = (DateTime.Now - t.Fecha_Nac).Days / 365
            });
            if (bolsa) {
                trabajadores = trabajadores.Where (t => t.EstadoTrabajador == Estados.Bolsa);
            }
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
            if (!string.IsNullOrEmpty (PerfilOcupacional)) {
                trabajadores = trabajadores.Where (t => t.PerfilOcupacional.ToString ().Equals (PerfilOcupacional));
            }
            if (!string.IsNullOrEmpty (Municipio)) {
                trabajadores = trabajadores.Where (t => t.MunicipioProv.ToString ().Equals (Municipio));
            }
            if (string.IsNullOrEmpty (EdadDesde)) {
                EdadDesde = "0";
            }
            if (string.IsNullOrEmpty (EdadHasta)) {
                EdadHasta = "300";
            }
            if (!string.IsNullOrEmpty (EdadDesde) || !string.IsNullOrEmpty (EdadHasta)) {
                trabajadores = trabajadores.Where (t => t.Edad >= int.Parse (EdadDesde) && t.Edad <= int.Parse (EdadHasta));
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
            var trab = context.Bolsa.Include (b => b.Trabajador).Where (s => s.Trabajador.EstadoTrabajador == Estados.Bolsa)
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
                        PerfilOcupacional = t.Trabajador.PerfilOcupacional.Nombre,
                        Municipio = t.Trabajador.Municipio.Nombre,
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
                        Fecha_Nac = t.Trabajador.Fecha_Nac.ToString ("dd MMMM yyyy"),
                        Edad = (DateTime.Now - t.Trabajador.Fecha_Nac).Days / 365,
                        nombre_Referencia = t.Nombre_Referencia,
                        Fecha_Entrada = t.Fecha.ToString ("dd MMMM yyyy"),
                        Tiempo_Bolsa = (DateTime.Now - t.Fecha).Days + " Días",
                });

            if (trab == null) {
                return NotFound ();
            }
            return Ok (trab);
        }
        // GET recursos_humanos/Trabajadores/Municipios
        [HttpGet ("/recursos_humanos/Trabajadores/Municipios")]
        public IEnumerable<Municipio> GetAllMunicipios () {
            return context.Municipio.ToList ();
        }

        // GET recursos_humanos/Trabajadores/ByUserId
        [HttpGet ("/recursos_humanos/Trabajadores/ByUserName")]
        public async Task<IActionResult> GetAllTrabUserNAme (bool tieneUsername) {
            // var url = "https://localhost:5001/auth/Account/usuarios";
            // var http = new HttpClient ();
            // var response = await http.GetStringAsync (url);
            // var users = JsonConvert.DeserializeObject<List<AccountDto>> (response);

            if (tieneUsername) {
                var trabajadores = context.Trabajador.Where (t => t.Username != null).Select (t => new {
                    Id = t.Id,
                        Nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase (t.Nombre),
                        Apellidos = t.Apellidos,
                        NombreCompleto = CultureInfo.InvariantCulture.TextInfo.ToTitleCase (t.Nombre + " " + t.Apellidos),
                        CI = t.CI,
                        Username = t.Username
                }).ToList ();
                return Ok (trabajadores);
            } else {
                return Ok (context.Trabajador.Where (t => t.Username == null).ToList ());
            }
        }
        // POST recursos_humanos/Trabajadores/AddUserName
        [HttpPost ("/recursos_humanos/Trabajadores/AddUserName")]
        public IActionResult AddUserName (List<UserTrabajador> userTrabajadorDto) {
            var trabajadores = context.Trabajador;
            foreach (var item in userTrabajadorDto) {
                var trab = trabajadores.FirstOrDefault (x => x.Username == item.Username);
                if (trab == null) {
                    var t = trabajadores.Find (item.TrabajadorId);
                    t.Username = item.Username;
                    context.Update (t);
                } else {
                    return BadRequest ($"Ya el usuario " + item.Username + " está asignado al trabajador " + trab.NombreCompleto);
                }
            }
            context.SaveChanges ();
            return Ok ();
        }
        // PUT recursos_humanos/Trabajadores/NullUserName
        [HttpPut ("/recursos_humanos/Trabajadores/NullUserName/{id}")]
        public IActionResult NullUserId ([FromBody] TrabajadorDto trabajadorDto, int id) {
            var t = context.Trabajador.Find (id);
            t.Username = null;
            context.Update (t);
            context.SaveChanges ();
            return Ok ();
        }
        private bool TrabajadorExists (int id) {
            return context.Trabajador.Any (e => e.Id == id);
        }
    }
}