using System.Collections.Generic;
using System.Linq;
using System.Net;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class EntidadesController : Controller {
        private readonly ContratacionDbContext context;
        public EntidadesController (ContratacionDbContext context) {
            this.context = context;
        }

        // GET entidades/Entidades
        [HttpGet]
        public IActionResult GetAll () {
            var entidades = context.Entidades.Include (e => e.CuentasBancarias).Include (e => e.Telefonos).Select (e => new {
                Id = e.Id,
                    Nombre = e.Nombre,
                    Siglas = e.Siglas,
                    Codigo = e.Codigo,
                    Direccion = e.Direccion,
                    Nit = e.Nit,
                    Fax = e.Fax,
                    Sector = e.Sector,
                    SectorNombre = e.Sector.ToString (),
                    Correo = e.Correo,
                    ObjetoSocial = e.ObjetoSocial,
                    Telefonos = e.Telefonos,
                    CantTelefonos = e.Telefonos.Count (),
                    CuentasBancarias = e.CuentasBancarias.Select (c => new {
                        Id = c.Id,
                            NumeroCuenta = c.NumeroCuenta,
                            NumeroSucursal = c.NumeroSucursal,
                            NombreSucursal = c.NombreSucursal,
                            NombreSucursalString = c.NombreSucursal.ToString (),
                            Moneda = c.Moneda,
                            MonedaString = c.Moneda.ToString ()
                    }),
                    CantCuentas = e.CuentasBancarias.Count ()
            });

            return Ok (entidades);
        }
        // GET: entidades/Entidades/Id
        [HttpGet ("{id}", Name = "GetEntidad")]
        public IActionResult GetbyId (int id) {
            var entidad = context.Entidades.FirstOrDefault (s => s.Id == id);

            if (entidad == null) {
                return NotFound ();
            }
            return Ok (entidad);
        }

        // POST entidades/Entidades
        [HttpPost]
        public IActionResult POST ([FromBody] Entidad entidad) {
            if (ModelState.IsValid) {
                foreach (var item in entidad.CuentasBancarias) {
                    if (context.CuentasBancarias.Any (c => c.NumeroCuenta == item.NumeroCuenta)) {
                        return BadRequest ($"Ya hay un proveedor con el número de cuenta bancaria: " + item.NumeroCuenta);
                    }
                }
                if (context.Entidades.Any (c => c.Nit == entidad.Nit)) {
                    return BadRequest ($"Ya hay un proveedor con este NIT: " + entidad.Nit);
                } else {
                    var ent = new Entidad {
                        Id = entidad.Id,
                        Nombre = entidad.Nombre,
                        Siglas = entidad.Siglas,
                        Codigo = entidad.Codigo,
                        Direccion = entidad.Direccion,
                        Nit = entidad.Nit,
                        Fax = entidad.Fax,
                        Sector = entidad.Sector,
                        Correo = entidad.Correo,
                        ObjetoSocial = entidad.ObjetoSocial
                    };
                    context.Entidades.Add (ent);

                    if (entidad.Telefonos != null) {
                        foreach (var item in entidad.Telefonos) {
                        if (item.Numero != null) {
                        var telefono = new Telefono {
                        Id = item.Id,
                        Numero = item.Numero,
                        Extension = item.Extension,
                        EntidadId = ent.Id
                                };
                                context.Telefonos.Add (telefono);
                            }

                        }
                    }
                    if (entidad.CuentasBancarias != null) {
                        foreach (var item in entidad.CuentasBancarias) {
                        if (item.NumeroCuenta != null) {
                        var cuenta = new CuentaBancaria {
                        Id = item.Id,
                        NumeroCuenta = item.NumeroCuenta,
                        NumeroSucursal = item.NumeroSucursal,
                        NombreSucursal = item.NombreSucursal,
                        Moneda = item.Moneda,
                        EntidadId = ent.Id
                                };
                                context.CuentasBancarias.Add (cuenta);
                            }
                        }
                    }
                    context.SaveChanges ();
                    return new CreatedAtRouteResult ("GetEntidad", new { id = ent.Id });
                }
            }
            return BadRequest (ModelState);
        }

        // PUT entidades/entidad/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Entidad entidad, int id) {
            var ent = context.Entidades.FirstOrDefault (s => s.Id == id);

            ent.Nombre = entidad.Nombre;
            ent.Siglas = entidad.Siglas;
            ent.Codigo = entidad.Codigo;
            ent.Direccion = entidad.Direccion;
            ent.Nit = entidad.Nit;
            ent.Sector = entidad.Sector;
            ent.Fax = entidad.Fax;
            ent.Correo = entidad.Correo;
            ent.ObjetoSocial = entidad.ObjetoSocial;
            context.Entry (ent).State = EntityState.Modified;

            var telefonos = context.Telefonos.Where (t => t.EntidadId == ent.Id);
            if (telefonos != null) {
                foreach (var item in telefonos) {
                    var telef = context.Telefonos.FirstOrDefault (s => s.Id == item.Id);
                    context.Telefonos.Remove (telef);
                    context.SaveChanges ();
                }
            }
            foreach (var item in entidad.Telefonos) {
                if (item.Numero != null) {
                    var telefono = new Telefono {
                    Id = item.Id,
                    Numero = item.Numero,
                    Extension = item.Extension,
                    EntidadId = ent.Id
                    };
                    context.Telefonos.Add (telefono);
                }
            }
            var cuentas = context.CuentasBancarias.Where (c => c.EntidadId == ent.Id);
            if (cuentas != null) {
                foreach (var item in cuentas) {
                    var c = context.CuentasBancarias.FirstOrDefault (s => s.Id == item.Id);
                    context.CuentasBancarias.Remove (c);
                    context.SaveChanges ();
                }
            }
            if (entidad.CuentasBancarias != null) {
                foreach (var item in entidad.CuentasBancarias) {
                if (item.NumeroCuenta != null) {
                var cuenta = new CuentaBancaria {
                Id = item.Id,
                NumeroCuenta = item.NumeroCuenta,
                NumeroSucursal = item.NumeroSucursal,
                NombreSucursal = item.NombreSucursal,
                Moneda = item.Moneda,
                EntidadId = ent.Id
                        };
                        context.CuentasBancarias.Add (cuenta);
                    }
                }
            }
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE entidades/entidad/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var entidad = context.Entidades.FirstOrDefault (s => s.Id == id);
            if (entidad.Id != id) {
                return NotFound ();
            }
            if (context.Contratos.FirstOrDefault (c => c.EntidadId == id) != null) {
                return BadRequest ($"Este proveedor esta asociado a un contrato");
            }
            var telefonos = context.Telefonos.Where (t => t.EntidadId == id);
            var cuentas = context.CuentasBancarias.Where (c => c.EntidadId == id);
            foreach (var item in telefonos) {
                context.Telefonos.Remove (item);
            }
            foreach (var item in cuentas) {
                context.CuentasBancarias.Remove (item);
            }
            context.Entidades.Remove (entidad);
            context.SaveChanges ();
            return Ok (entidad);
        }
        // GET: contratacion/Entidades/Sucursales
        [HttpGet ("/contratacion/Entidades/Sucursales")]
        public IActionResult GetAllSucursales () {
            var tipo = new List<dynamic> () {
                new { Id = NombreSucursal.BPA, Nombre = "BPA" },
                new { Id = NombreSucursal.Bandec, Nombre = "Bandec" },
                new { Id = NombreSucursal.Banco_Metropolitano, Nombre = "Banco Metropolitano" },
                new { Id = NombreSucursal.BFI, Nombre = "BFI" },
            };
            return Ok (tipo);
        }
        // GET: contratacion/Entidades/Monedas
        [HttpGet ("/contratacion/Entidades/Monedas")]
        public IActionResult GetAllMonedas () {
            var tipo = new List<dynamic> () {
                new { Id = Moneda.CUP, Nombre = "CUP" },
                new { Id = Moneda.CUC, Nombre = "CUC" },
                new { Id = Moneda.USD, Nombre = "USD" },
            };
            return Ok (tipo);
        }
        // GET: contratacion/Entidades/Monedas
        [HttpGet ("/contratacion/Entidades/Monedas/{id}")]
        public IActionResult GetMonedasEntidad (int id) {
            var tipo = context.CuentasBancarias.Where (x => x.EntidadId == id).Select (m => new {
                Nombre = m.Moneda.ToString ()
            });
            return Ok (tipo);
        }
        // GET: contratacion/Entidades/Monedas
        [HttpGet ("/contratacion/Entidades/Sectores")]
        public IActionResult GetAllSectores () {
            var tipo = new List<dynamic> () {
                new { Id = Sector.Sin_Definir, Nombre = "Sin Definir" },
                new { Id = Sector.Estatal, Nombre = "Estatal" },
                new { Id = Sector.Cooperativo, Nombre = "Cooperativo" },
                new { Id = Sector.TCP, Nombre = "TCP" },
                new { Id = Sector.PYME, Nombre = "PYME" },
            };
            return Ok (tipo);
        }
    }
}