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
    public class ModelosController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public ModelosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/Modelos
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
        public ActionResult<PaginadorGenerico<Modelo>> GetModelos (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Modelo> _Modelos;
            PaginadorGenerico<Modelo> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Modelos = context.Modelos.Include (m => m.Marca).ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    _Modelos = _Modelos.Where (x => x.Nombre.Contains (item)).ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Modelos = _Modelos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Modelos = _Modelos.OrderBy (x => x.Id).ToList ();
                    break;

                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _Modelos = _Modelos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Modelos = _Modelos.OrderBy (x => x.Nombre).ToList ();
                    break;

                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _Modelos = _Modelos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Modelos = _Modelos.OrderBy (x => x.Nombre).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Modelos = _Modelos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Modelos = _Modelos.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Modelos
            _TotalRegistros = _Modelos.Count ();
            // Obtenemos la 'página de registros' de la tabla Modelos
            _Modelos = _Modelos.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Modelos
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<Modelo> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = _Modelos
            };
            return _Paginador;
        }
        // GET TallerWebApi/Modelos
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var modelos = context.Modelos.Where (m => m.Activo == true).Select (m => new {
                Id = m.Id,
                    Nombre = m.Nombre
            }).ToList ();
            return Ok (modelos);
        }

        // GET: TallerWebApi/Modelos/Id
        [HttpGet ("{id}", Name = "GetModelo")]
        public IActionResult GetbyId (int id) {
            var modelo = context.Modelos.FirstOrDefault (s => s.Id == id);
            if (modelo == null) {
                return NotFound ();
            }
            return Ok (modelo);
        }

        // POST TallerWebApi/Modelos
        [HttpPost]
        public IActionResult POST ([FromBody] ModeloDto modelo) {
            var m = new Modelo {
                Id = modelo.Id,
                Nombre = modelo.Nombre,
                MarcaId = modelo.Marca,
                Activo = modelo.Activo
            };
            if (ModelState.IsValid) {
                context.Modelos.Add (m);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetModelo", new { id = m.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Modelos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] ModeloDto modelo, int id) {
            if (modelo.Id != id) {
                return BadRequest (ModelState);

            }
            var m = new Modelo {
                Id = modelo.Id,
                Nombre = modelo.Nombre,
                MarcaId = modelo.Marca,
                Activo = modelo.Activo
            };
            context.Entry (m).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Modelos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var modelo = context.Modelos.FirstOrDefault (s => s.Id == id);

            if (modelo.Id != id) {
                return NotFound ();
            }
            context.Modelos.Remove (modelo);
            context.SaveChanges ();
            return Ok (modelo);
        }
    }
}