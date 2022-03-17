using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerWebApi.Data;
using TallerWebApi.Models;

namespace TallerWebApi.Controllers {
    [Route ("taller/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public ClientesController (TallerWebApiDbContext context) {
            this.context = context;
        }

        // GET: api/Clientes
        /// <summary>
        /// Obtiene un resultado paginado de los objetos de la BD.
        /// </summary>
        /// <param name="search">Texto de búsqueda.</param>
        /// <param name="order">Nombre de campo por el cual ordenar (distingue mayúsculas).</param>
        /// <param name="typeOrder">Tipo de orden: ASC (ascendente) / DESC (descendente).</param>
        /// <param name="page">Número de página a obtener.</param>
        /// <param name="itemsPerPage">Número de registros por página.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PaginadorGenerico<Cliente>>> GetClientes (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Cliente> _Clientes;
            PaginadorGenerico<Cliente> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Clientes = context.Clientes.ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    _Clientes = _Clientes.Where (x => x.Nombre.Contains (item) ||
                            x.Direccion.Contains (item) ||
                            x.Telefono.Contains (item) ||
                            x.Correo.Contains (item))
                        .ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Clientes = _Clientes.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Clientes = _Clientes.OrderBy (x => x.Id).ToList ();
                    break;

                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _Clientes = _Clientes.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Clientes = _Clientes.OrderBy (x => x.Nombre).ToList ();
                    break;

                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _Clientes = _Clientes.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Clientes = _Clientes.OrderBy (x => x.Nombre).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Clientes = _Clientes.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Clientes = _Clientes.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Clientes
            _TotalRegistros = _Clientes.Count ();
            // Obtenemos la 'página de registros' de la tabla Clientes
            _Clientes = _Clientes.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Clientes
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<Cliente> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = _Clientes
            };
            return _Paginador;
        }

        // GET TallerWebApi/Clientes
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var clientes = context.Clientes.Where (m => m.Activo == true).Select (m => new {
                Id = m.Id,
                    Nombre = m.Nombre
            }).ToList ();
            return Ok (clientes);
        }

        // GET: TallerWebApi/Clientes/Id
        [HttpGet ("{id}", Name = "GetCliente")]
        public IActionResult GetbyId (int id) {
            var cliente = context.Clientes.FirstOrDefault (s => s.Id == id);
            if (cliente == null) {
                return NotFound ();
            }
            return Ok (cliente);
        }

        // POST TallerWebApi/Clientes
        [HttpPost]
        public IActionResult POST ([FromBody] Cliente cliente) {
            if (ModelState.IsValid) {
                if (cliente.Sexo != null) {
                    var sexo = new SexoCliente ();
                    IFormatProvider culture = new System.Globalization.CultureInfo ("en-US", true);
                    if (cliente.CI != null) {
                        var sexoCI = int.Parse (cliente.CI.Substring (9, 1));
                        if (sexoCI % 2 == 0) {
                            sexo = SexoCliente.M;
                        } else {
                            sexo = SexoCliente.F;
                        }
                    }
                    cliente.Sexo = sexo;
                }
                context.Clientes.Add (cliente);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetCliente", new { id = cliente.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Clientes/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Cliente cliente, int id) {
            if (cliente.Id != id) {
                return BadRequest (ModelState);
            }
            if (cliente.Sexo != null) {
                var sexo = new SexoCliente ();
                IFormatProvider culture = new System.Globalization.CultureInfo ("en-US", true);
                if (cliente.CI != null) {
                    var sexoCI = int.Parse (cliente.CI.Substring (9, 1));
                    if (sexoCI % 2 == 0) {
                        sexo = SexoCliente.M;
                    } else {
                        sexo = SexoCliente.F;
                    }
                }
                cliente.Sexo = sexo;
            }
            context.Entry (cliente).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Clientes/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var cliente = context.Clientes.FirstOrDefault (s => s.Id == id);

            if (cliente.Id != id) {
                return NotFound ();
            }
            context.Clientes.Remove (cliente);
            context.SaveChanges ();
            return Ok (cliente);
        }
    }
}