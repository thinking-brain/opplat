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
    public class EquiposController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public EquiposController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/Equipos
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
        public ActionResult<PaginadorGenerico<Equipo>> GetEquipos (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            List<Equipo> _Equipos;
            PaginadorGenerico<Equipo> _PaginadorEquipos;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            _Equipos = context.Equipos
                .Include (e => e.TipoEquipo)
                .Include (e => e.Marca)
                .Include (e => e.Modelo)
                .Include (e => e.Cliente)
                .Include (e => e.Documentos)
                .ToList ();

            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                // foreach (var item in search.Split (new char[] { ' ' },
                //         StringSplitOptions.RemoveEmptyEntries)) {
                //     _Equipos = _Equipos.Where (x => x.Marca.Contains (item)).ToList ();
                // }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        _Equipos = _Equipos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Equipos = _Equipos.OrderBy (x => x.Id).ToList ();
                    break;
                case "Name":
                    if (typeOrder.ToLower () == "desc")
                        _Equipos = _Equipos.OrderByDescending (x => x.NumeroSerie).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Equipos = _Equipos.OrderBy (x => x.NumeroSerie).ToList ();
                    break;
                case "ContactName":
                    if (typeOrder.ToLower () == "desc")
                        _Equipos = _Equipos.OrderByDescending (x => x.NumeroSerie).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Equipos = _Equipos.OrderBy (x => x.NumeroSerie).ToList ();
                    break;

                    // ...
                    // Aquí el resto de los campos de la tabla por los que ordenar.
                    // ...

                default:
                    if (typeOrder.ToLower () == "desc")
                        _Equipos = _Equipos.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        _Equipos = _Equipos.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla Equipos
            _TotalRegistros = _Equipos.Count ();
            // Obtenemos la 'página de registros' de la tabla Equipos
            _Equipos = _Equipos.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla Equipos
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _PaginadorEquipos = new PaginadorGenerico<Equipo> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = (IEnumerable<Equipo>) _Equipos
            };
            return _PaginadorEquipos;
        }

        // GET TallerWebApi/Equipos
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var equipos = context.Equipos.Include (m => m.Marca).Include (m => m.Modelo).Where (m => m.Activo == true).Select (m => new {
                Id = m.Id,
                    Marca = m.Marca,
                    Modelo = m.Modelo
            }).ToList ();
            return Ok (equipos);
        }

        // GET: TallerWebApi/Equipos/Id
        [HttpGet ("{id}", Name = "GetEquipo")]
        public IActionResult GetbyId (int id) {
            var equipo = context.Equipos.Include (e => e.TipoEquipo)
                .Include (e => e.Marca)
                .Include (e => e.Modelo)
                .Include (e => e.Cliente).FirstOrDefault (s => s.Id == id);
            if (equipo == null) {
                return NotFound ();
            }
            return Ok (equipo);
        }

        // POST TallerWebApi/Equipos
        [HttpPost]
        public IActionResult POST ([FromBody] EquipoDto equipo) {
            var e = new Equipo {
                Id = equipo.Id,
                NumeroSerie = equipo.NumeroSerie,
                FechaFabricacion = equipo.FechaFabricacion,
                TipoEquipoId = equipo.TipoEquipo,
                EstadoEquipo = equipo.EstadoEquipo,
                ClienteId = equipo.Cliente,
                MarcaId = equipo.Marca,
                ModeloId = equipo.Modelo,
                Observaciones = equipo.Observaciones,
                Activo = equipo.Activo
            };
            if (ModelState.IsValid) {
                context.Equipos.Add (e);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetEquipo", new { id = e.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Equipos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] EquipoDto equipo, int id) {
            if (equipo.Id != id) {
                return BadRequest (ModelState);
            }
            var e = new Equipo {
                Id = equipo.Id,
                NumeroSerie = equipo.NumeroSerie,
                FechaFabricacion = equipo.FechaFabricacion,
                TipoEquipoId = equipo.TipoEquipo,
                EstadoEquipo = equipo.EstadoEquipo,
                ClienteId = equipo.Cliente,
                MarcaId = equipo.Marca,
                ModeloId = equipo.Modelo,
                Observaciones = equipo.Observaciones,
                Activo = equipo.Activo
            };
            context.Entry (e).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Equipos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var equipo = context.Equipos.FirstOrDefault (s => s.Id == id);

            if (equipo.Id != id) {
                return NotFound ();
            }
            context.Equipos.Remove (equipo);
            context.SaveChanges ();
            return Ok (equipo);
        }
        // GET: contratacion/contratos/Tipos
        [HttpGet ("EstadoEquipo")]
        public IActionResult GetSituacionEquipo () {
            var situacion = new List<dynamic> () {
                new { Id = EstadoEquipo.Ninguno, Nombre = "Ninguno" },
                new { Id = EstadoEquipo.Dañado, Nombre = "Dañado" },
                new { Id = EstadoEquipo.Sin_Reparacion, Nombre = "Sin Reparacion" },
                new { Id = EstadoEquipo.Ok, Nombre = "Ok" },
            };
            return Ok (situacion);
        }
    }
}