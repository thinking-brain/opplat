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
    public class TiposEquiposController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public TiposEquiposController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/TipoEquipos
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
        public ActionResult<PaginadorGenerico<TipoEquipo>> GetTiposEquipos (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<TipoEquipo> _TiposEquipos;
            PaginadorGenerico<TipoEquipo> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _TiposEquipos = context.TiposEquipos.ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    _TiposEquipos = _TiposEquipos.Where (x => x.Nombre.Contains (item) || x.Codigo.Contains (item)).ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _TiposEquipos = _TiposEquipos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _TiposEquipos = _TiposEquipos.OrderBy (x => x.Id).ToList ();
                    break;

                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _TiposEquipos = _TiposEquipos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _TiposEquipos = _TiposEquipos.OrderBy (x => x.Nombre).ToList ();
                    break;

                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _TiposEquipos = _TiposEquipos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _TiposEquipos = _TiposEquipos.OrderBy (x => x.Nombre).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _TiposEquipos = _TiposEquipos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _TiposEquipos = _TiposEquipos.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla TipoEquipos
            _TotalRegistros = _TiposEquipos.Count ();
            // Obtenemos la 'página de registros' de la tabla TipoEquipos
            _TiposEquipos = _TiposEquipos.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla TipoEquipos
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<TipoEquipo> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = _TiposEquipos
            };
            return _Paginador;
        }
        // GET TallerWebApi/TipoEquipos
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var tipos = context.TiposEquipos.Where (m => m.Activo == true).Select (m => new {
                Id = m.Id,
                    Nombre = m.Nombre
            }).ToList ();
            return Ok (tipos);
        }

        // GET: TallerWebApi/TiposEquipos/Id
        [HttpGet ("{id}", Name = "GetTipoEquipo")]
        public IActionResult GetbyId (int id) {
            var tipo = context.TiposEquipos.FirstOrDefault (s => s.Id == id);
            if (tipo == null) {
                return NotFound ();
            }
            return Ok (tipo);
        }

        // POST TallerWebApi/TiposEquipos
        [HttpPost]
        public IActionResult POST ([FromBody] TipoEquipo tipo) {
            if (ModelState.IsValid) {
                context.TiposEquipos.Add (tipo);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTipoEquipo", new { id = tipo.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/TiposEquipos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] TipoEquipo tipo, int id) {
            if (tipo.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (tipo).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/TiposEquipos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var tipo = context.TiposEquipos.FirstOrDefault (s => s.Id == id);

            if (tipo.Id != id) {
                return NotFound ();
            }
            context.TiposEquipos.Remove (tipo);
            context.SaveChanges ();
            return Ok (tipo);
        }
    }
}