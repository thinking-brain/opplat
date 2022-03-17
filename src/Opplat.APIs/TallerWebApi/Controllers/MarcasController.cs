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
    public class MarcasController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public MarcasController (TallerWebApiDbContext context) {
            this.context = context;
        }

        // GET: api/Marcas
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
        public ActionResult<PaginadorGenerico<Marca>> GetMarcas (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Marca> _Marcas;
            PaginadorGenerico<Marca> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Marcas = context.Marcas.ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    _Marcas = _Marcas.Where (x => x.Nombre.Contains (item)).ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Marcas = _Marcas.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Marcas = _Marcas.OrderBy (x => x.Id).ToList ();
                    break;

                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _Marcas = _Marcas.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Marcas = _Marcas.OrderBy (x => x.Nombre).ToList ();
                    break;

                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _Marcas = _Marcas.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Marcas = _Marcas.OrderBy (x => x.Nombre).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Marcas = _Marcas.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Marcas = _Marcas.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Marcas
            _TotalRegistros = _Marcas.Count ();
            // Obtenemos la 'página de registros' de la tabla Marcas
            _Marcas = _Marcas.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Marcas
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<Marca> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = _Marcas
            };
            return _Paginador;
        }
        // GET TallerWebApi/Marcas
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var marcas = context.Marcas.Where (m => m.Activo == true).ToList ();
            return Ok (marcas);
        }

        // GET: TallerWebApi/Marcas/Id
        [HttpGet ("{id}", Name = "GetMarca")]
        public IActionResult GetbyId (int id) {
            var marca = context.Marcas.FirstOrDefault (s => s.Id == id);
            if (marca == null) {
                return NotFound ();
            }
            return Ok (marca);
        }

        // POST TallerWebApi/Marcas
        [HttpPost]
        public IActionResult POST ([FromBody] Marca marca) {
            if (ModelState.IsValid) {
                context.Marcas.Add (marca);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetMarca", new { id = marca.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Marcas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Marca marca, int id) {
            if (marca.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (marca).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Marcas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var marca = context.Marcas.FirstOrDefault (s => s.Id == id);

            if (marca.Id != id) {
                return NotFound ();
            }
            context.Marcas.Remove (marca);
            context.SaveChanges ();
            return Ok (marca);
        }
    }
}