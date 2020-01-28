using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;
using RhWebApi.Dtos;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class CaracteristicasTrabController : Controller {
        private readonly RhWebApiDbContext context;
        public CaracteristicasTrabController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET: recursos_humanos/CaracteristicasTrab/Sexo
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/Sexo")]
        public IActionResult GetAllSexo () {
            var sexo = new List<dynamic> () {
                new { Id = Sexo.M, Nombre = Sexo.M.ToString () },
                new { Id = Sexo.F, Nombre = Sexo.F.ToString () },
            };
            return Ok (sexo);
        }
        // GET: recursos_humanos/CaracteristicasTrab/ColordePiel
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/ColordePiel")]
        public IActionResult GetAllColorDePiel () {
            var colorDePiel = new List<dynamic> () {
                new { Id = ColorDePiel.Blanca, Nombre = ColorDePiel.Blanca.ToString () },
                new { Id = ColorDePiel.Negra, Nombre = ColorDePiel.Negra.ToString () },
                new { Id = ColorDePiel.Mestiza, Nombre = ColorDePiel.Mestiza.ToString () },
            };
            return Ok (colorDePiel);
        }
        // GET: recursos_humanos/CaracteristicasTrab/ColordePiel
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/ColordeOjos")]
        public IActionResult GetAllColorDeOjos () {
            var colorDeOjos = new List<dynamic> () {
                new { Id = ColorDeOjos.Azules, Nombre = ColorDeOjos.Azules.ToString () },
                new { Id = ColorDeOjos.Verdes, Nombre = ColorDeOjos.Verdes.ToString () },
                new { Id = ColorDeOjos.Negros, Nombre = ColorDeOjos.Negros.ToString () },
                new { Id = ColorDeOjos.Marron, Nombre = ColorDeOjos.Marron.ToString () },
                new { Id = ColorDeOjos.Pardos, Nombre = ColorDeOjos.Pardos.ToString () },
            };
            return Ok (colorDeOjos);
        }
         // GET: recursos_humanos/CaracteristicasTrab/ColordePiel
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/TallaDeCamisa")]
        public IActionResult GetAllTallaDeCamisa () {
            var tallaDeCamisa = new List<dynamic> () {
                new { Id = TallaDeCamisa.S, Nombre = TallaDeCamisa.S.ToString () },
                new { Id = TallaDeCamisa.M, Nombre = TallaDeCamisa.M.ToString () },
                new { Id = TallaDeCamisa.L, Nombre = TallaDeCamisa.L.ToString () },
                new { Id = TallaDeCamisa.X, Nombre = TallaDeCamisa.X.ToString () },
                new { Id = TallaDeCamisa.XL, Nombre = TallaDeCamisa.XL.ToString () },
                new { Id = TallaDeCamisa.XXL, Nombre = TallaDeCamisa.XXL.ToString () },
                new { Id = TallaDeCamisa.XXXL, Nombre = TallaDeCamisa.XXXL.ToString () },
            };
            return Ok (tallaDeCamisa);
        }
        // GET: recursos_humanos/CaracteristicasTrab/Estados
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/Estados")]
        public IActionResult GetAllEstados () {
            var colorDePiel = new List<dynamic> () {
                new { Id = Estados.Activo, Nombre = Estados.Activo.ToString () },
                new { Id = Estados.Baja, Nombre = Estados.Baja.ToString () },
                new { Id = Estados.Interrupto, Nombre = Estados.Interrupto.ToString () },
                new { Id = Estados.Disponible, Nombre = Estados.Disponible.ToString () },
                new { Id = Estados.Licencia_Maternidad, Nombre = Estados.Licencia_Maternidad.ToString () },
                new { Id = Estados.Certificado, Nombre = Estados.Certificado.ToString () },
            };
            return Ok (colorDePiel);
        }
        // GET: recursos_humanos/CaracteristicasTrab/NivelEscolaridad
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/NivelEscolaridad")]
        public IActionResult GetAllEscolaridad () {
            var nivelEscolaridad = new List<dynamic> () {
                new { Id = NivelDeEscolaridad.TecnicoMedio, Nombre = NivelDeEscolaridad.TecnicoMedio.ToString () },
                new { Id = NivelDeEscolaridad.NivelSuperior, Nombre = NivelDeEscolaridad.NivelSuperior.ToString () },
                new { Id = NivelDeEscolaridad.DoceGrado, Nombre = NivelDeEscolaridad.DoceGrado.ToString () },
                new { Id = NivelDeEscolaridad.NovenoGrado, Nombre = NivelDeEscolaridad.NovenoGrado.ToString () },
                new { Id = NivelDeEscolaridad.SextoGrado, Nombre = NivelDeEscolaridad.SextoGrado.ToString () },
                new { Id = NivelDeEscolaridad.MenosDeSextoGrado, Nombre = NivelDeEscolaridad.MenosDeSextoGrado.ToString () },
            };
            return Ok (nivelEscolaridad);
        }

        // GET: recursos_humanos/CaracteristicasTrab/Id
        [HttpGet ("{id}", Name = "GetCaracteristicasTrab")]
        public IActionResult GetbyId (int id) {
            var caractTrab = context.CaracteristicasTrab.Where (s => s.TrabajadorId == id)
                .Select (t => new {
                    Trabajador = t.Trabajador.Nombre + " " + t.Trabajador.Apellidos,
                        ColorDePiel = t.ColorDePiel.ToString (),
                        ColorDeOjos = t.ColorDeOjos.ToString (),
                        TallaDeCamisa = t.TallaDeCamisa.ToString (),
                        Resumen = t.Resumen,
                });

            if (caractTrab == null) {
                return NotFound ();
            }
            return Ok (caractTrab);

        }

        // POST recursos_humanos/CaracteristicasTrab
        [HttpPost]
        public IActionResult POST ([FromBody] CaracteristicasTrabDto caractTrabDto) {
            if (ModelState.IsValid) {
                var caractTrab = new CaracteristicasTrab () {
                    TrabajadorId = caractTrabDto.TrabajadorId,
                    ColorDePiel = caractTrabDto.ColorDePiel,
                    ColorDeOjos = caractTrabDto.ColorDeOjos,
                    TallaDeCamisa = caractTrabDto.TallaDeCamisa,
                    TallaPantalon = caractTrabDto.TallaPantalon,
                    TallaCalzado = caractTrabDto.TallaCalzado,
                    OtrasCaracteristicas = caractTrabDto.OtrasCaracteristicas,
                };
                context.CaracteristicasTrab.Add (caractTrab);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetCaracteristicasTrab", new { id = caractTrabDto.TrabajadorId });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/CaracteristicasTrab/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] CaracteristicasTrab caractTrab, int id) {
            if (caractTrab.TrabajadorId != id) {
                return BadRequest (ModelState);
            }
            context.Entry (caractTrab).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/CaracteristicasTrab/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var caractTrab = context.CaracteristicasTrab.FirstOrDefault (s => s.TrabajadorId == id);

            if (caractTrab.TrabajadorId != id) {
                return NotFound ();
            }
            context.CaracteristicasTrab.Remove (caractTrab);
            context.SaveChanges ();
            return Ok (caractTrab);
        }
    }
}