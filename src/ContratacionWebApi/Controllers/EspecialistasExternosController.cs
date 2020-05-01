using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
        [Route ("contratacion/[controller]")]
        [ApiController]
        public class EspecialistasExternosController : Controller {
            private readonly ContratacionDbContext context;
            public EspecialistasExternosController (ContratacionDbContext context) {
                this.context = context;
            }

            // GET EspecialistasExternos/EspecialistasExternos
            [HttpGet]
            public ActionResult GetAll () {
                var especialistasExternos = context.EspecialistasExternos.Select (e => new {
                        Id = e.Id,
                            Nombre = e.Nombre,
                            Apellidos = e.Apellidos,
                            NombreCompleto=e.NombreCompleto,
                            EntidadId = e.EntidadId,
                            Entidad = e.Entidad.Nombre,
                            Area = e.Area,
                            Departamento = e.Departamento,
                            Cargo = e.Cargo,
                    });
                    return Ok (especialistasExternos);
                }

                // GET: EspecialistasExternos/EspecialistasExternos/Id
                [HttpGet ("{id}", Name = "GetespecialistaExterno")]
                public IActionResult GetbyId (int id) {
                    var especialistaExterno = context.EspecialistasExternos.FirstOrDefault (s => s.Id == id);

                    if (especialistaExterno == null) {
                        return NotFound ();
                    }
                    return Ok (especialistaExterno);
                }

                // POST EspecialistasExternos/EspecialistasExternos
                [HttpPost]
                public IActionResult POST ([FromBody] EspecialistaExternoDto especialistaExternoDto) {
                    if (ModelState.IsValid) {
                        var especialistaExterno = new EspecialistaExterno {
                            Id = especialistaExternoDto.Id,
                            Nombre = especialistaExternoDto.Nombre,
                            Apellidos = especialistaExternoDto.Apellidos,
                            EntidadId = especialistaExternoDto.EntidadId,
                            Area = especialistaExternoDto.Area,
                            Departamento = especialistaExternoDto.Departamento,
                            Cargo = especialistaExternoDto.Cargo,
                        };
                        context.EspecialistasExternos.Add (especialistaExterno);
                        context.SaveChanges ();
                        return new CreatedAtRouteResult ("GetespecialistaExterno", new { id = especialistaExterno.Id });
                    }
                    return BadRequest (ModelState);
                }

                // PUT EspecialistasExternos/especialistaExterno/id
                [HttpPut ("{id}")]
                public IActionResult PUT ([FromBody] EspecialistaExterno especialistaExterno, int id) {
                    if (especialistaExterno.Id != id) {
                        return BadRequest (ModelState);

                    }
                    context.Entry (especialistaExterno).State = EntityState.Modified;
                    context.SaveChanges ();
                    return Ok ();
                }

                // DELETE EspecialistasExternos/especialistaExterno/id
                [HttpDelete ("{id}")]
                public IActionResult Delete (int id) {
                    var especialistaExterno = context.EspecialistasExternos.FirstOrDefault (s => s.Id == id);

                    if (especialistaExterno.Id != id) {
                        return NotFound ();
                    }
                    context.EspecialistasExternos.Remove (especialistaExterno);
                    context.SaveChanges ();
                    return Ok (especialistaExterno);
                }
            }
        }