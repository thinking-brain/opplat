using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Data;
using RhWebApi.Models;

namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class CaracteristicasTrabController : Controller {
        private readonly RhWebApiDbContext context;
        public CaracteristicasTrabController (RhWebApiDbContext context) {
            this.context = context;
        }

        // // GET recursos_humanos/CaracteristicasTrab
        // [HttpGet]
        // public IEnumerable<CaracteristicasTrab> GetAll()
        // {                            
        //     return context.CaracteristicasTrab.ToList();            
        // }       

        // GET: recursos_humanos/CaracteristicasTrab/Id

        // GET recursos_humanos/CaracteristicasTrab/ColorOjos
        [HttpGet ("/recursos_humanos/CaracteristicasTrab/ColorOjos")]
        public string GetAllColorOjos () {
            // for (int i = 0; i < 5; i++) {
            //     ColorDeOjos colorDeOjos = (ColorDeOjos) i;
            //     return Ok (colorDeOjos.ToString ());
            // }
            // foreach (int i in Enum.GetValues (typeof (ColorDeOjos))) {
            //     return Ok (i.ToString());
            // }

            var stringBuilder = new StringBuilder ();
            foreach (string colorDeOjos in Enum.GetNames (typeof (ColorDeOjos))) {
                stringBuilder.Append (colorDeOjos + ",");
            }
            return stringBuilder.ToString ();
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