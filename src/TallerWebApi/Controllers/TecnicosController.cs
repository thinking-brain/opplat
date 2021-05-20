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
    public class TecnicosController : Controller {
        private readonly TallerWebApiDbContext context;
        public TecnicosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/Tecnicos
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
        public ActionResult<PaginadorGenerico<Tecnico>> GetTecnicos (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Tecnico> _Tecnicos;
            PaginadorGenerico<Tecnico> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Tecnicos = context.Tecnicos.Include (t => t.Taller).ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    _Tecnicos = _Tecnicos.Where (x => x.Nombre.Contains (item)).ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Tecnicos = _Tecnicos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Tecnicos = _Tecnicos.OrderBy (x => x.Id).ToList ();
                    break;

                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _Tecnicos = _Tecnicos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Tecnicos = _Tecnicos.OrderBy (x => x.Nombre).ToList ();
                    break;

                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _Tecnicos = _Tecnicos.OrderByDescending (x => x.Nombre).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Tecnicos = _Tecnicos.OrderBy (x => x.Nombre).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Tecnicos = _Tecnicos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Tecnicos = _Tecnicos.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Tecnicos
            _TotalRegistros = _Tecnicos.Count ();
            // Obtenemos la 'página de registros' de la tabla Tecnicos
            _Tecnicos = _Tecnicos.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Tecnicos
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<Tecnico> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = _Tecnicos
            };
            return _Paginador;
        }
        // GET TallerWebApi/Tecnicos
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var tecnicos = context.Tecnicos.Where (m => m.Activo == true).ToList ();
            return Ok (tecnicos);
        }

        // GET: TallerWebApi/Tecnicos/Id
        [HttpGet ("{id}", Name = "GetTecnico")]
        public IActionResult GetbyId (int id) {
            var tecnico = context.Tecnicos.FirstOrDefault (s => s.Id == id);
            if (tecnico == null) {
                return NotFound ();
            }
            return Ok (tecnico);
        }

        // POST TallerWebApi/Tecnicos
        [HttpPost]
        public IActionResult POST ([FromBody] TecnicoDto tecnico) {
            if (ModelState.IsValid) {
                var t = new Tecnico {
                    Id = tecnico.Id,
                    Nombre = tecnico.Nombre,
                    Cargo = tecnico.Cargo,
                    TallerId = tecnico.Taller,
                    Activo = tecnico.Activo
                };
                context.Tecnicos.Add (t);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTecnico", new { id = t.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Tecnicos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] TecnicoDto tecnico, int id) {
            if (tecnico.Id != id) {
                return BadRequest (ModelState);

            }
            var t = new Tecnico {
                Id = tecnico.Id,
                Nombre = tecnico.Nombre,
                Cargo = tecnico.Cargo,
                TallerId = tecnico.Taller,
                Activo = tecnico.Activo
            };
            context.Entry (t).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Tecnicos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var tecnico = context.Tecnicos.FirstOrDefault (s => s.Id == id);

            if (tecnico.Id != id) {
                return NotFound ();
            }
            context.Tecnicos.Remove (tecnico);
            context.SaveChanges ();
            return Ok (tecnico);
        }
    }
}