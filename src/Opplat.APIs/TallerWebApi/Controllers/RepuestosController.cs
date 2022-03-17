using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerWebApi.Data;
using TallerWebApi.Models;

namespace TallerWebApi.Controllers {
    [Route ("taller/[controller]")]
    [ApiController]
    public class RepuestosController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public RepuestosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/Repuesto
        /// <summary>
        /// Obtiene un resultado paginado de los objetos de la BD.
        /// </summary>
        /// <param name="search">Texto de búsqueda.</param>
        /// <param name="order">NumeroSerie de campo por el cual ordenar (distingue mayúsculas).</param>
        /// <param name="typeOrder">EstadoEquipo de orden: ASC (ascendente) / DESC (descendente).</param>
        /// <param name="page">Número de página a obtener.</param>
        /// <param name="itemsPerPage">Número de registros por página.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<PaginadorGenerico<Repuesto>> GetRepuesto (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Repuesto> _Repuesto;
            PaginadorGenerico<Repuesto> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Repuesto = context.Repuestos.ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                // foreach (var item in search.Split (new char[] { ' ' },
                //         StringSplitOptions.RemoveEmptyEntries)) {
                //     _Repuesto = _Repuesto.Where (x => x.Marca.Contains (item)).ToList ();
                // }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Repuesto = _Repuesto.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Repuesto = _Repuesto.OrderBy (x => x.Id).ToList ();
                    break;
                case "Nombre":
                    if (typeOrder.ToLower () == "desc")
                        _Repuesto = _Repuesto.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Repuesto = _Repuesto.OrderBy (x => x.Nombre).ToList ();
                    break;
                case "Precio":
                    if (typeOrder.ToLower () == "desc")
                        _Repuesto = _Repuesto.OrderByDescending (x => x.Precio).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Repuesto = _Repuesto.OrderBy (x => x.Precio).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Repuesto = _Repuesto.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Repuesto = _Repuesto.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Repuesto
            _TotalRegistros = _Repuesto.Count ();
            // Obtenemos la 'página de registros' de la tabla Repuesto
            _Repuesto = _Repuesto.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Repuesto
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<Repuesto> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = (IEnumerable<Repuesto>) _Repuesto
            };
            return _Paginador;
        }

        // GET TallerWebApi/Repuesto
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var repuestos = context.Repuestos.Where (m => m.Activo == true).Select (m => new {
                Id = m.Id,
                    Nombre = m.Nombre,
            }).ToList ();
            return Ok (repuestos);
        }

        // GET: TallerWebApi/Repuesto/Id
        [HttpGet ("{id}", Name = "GetRepuesto")]
        public IActionResult GetbyId (int id) {
            var repuesto = context.Repuestos.FirstOrDefault (s => s.Id == id);
            if (repuesto == null) {
                return NotFound ();
            }
            return Ok (repuesto);
        }

        // POST TallerWebApi/Repuesto
        [HttpPost]
        public IActionResult POST ([FromBody] Repuesto repuesto) {
            if (ModelState.IsValid) {
                context.Repuestos.Add (repuesto);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetRepuesto", new { id = repuesto.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Repuesto/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Repuesto repuesto, int id) {
            if (repuesto.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (repuesto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Repuesto/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var repuesto = context.Repuestos.FirstOrDefault (s => s.Id == id);

            if (repuesto.Id != id) {
                return NotFound ();
            }
            context.Repuestos.Remove (repuesto);
            context.SaveChanges ();
            return Ok (repuesto);
        }
    }
}