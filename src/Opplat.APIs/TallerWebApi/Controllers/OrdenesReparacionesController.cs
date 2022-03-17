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
    public class OrdenesReparacionController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public OrdenesReparacionController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET: api/OrdenesReparacion
        /// <summary>
        /// Obtiene un resultado paginado de los objetos de la BD.
        /// </summary>
        /// <param name="search">Texto de búsqueda.</param>
        /// <param name="order">NoOrden de campo por el cual ordenar (distingue mayúsculas).</param>
        /// <param name="typeOrder">Tipo de orden: ASC (ascendente) / DESC (descendente).</param>
        /// <param name="page">Número de página a obtener.</param>
        /// <param name="itemsPerPage">Número de registros por página.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetOrdenesReparacion (string search,
            string order = "Id",
            string typeOrder = "ASC",
            int page = 1,
            int itemsPerPage = 10) {
            PaginadorGenerico<OrdenReparacion> _Paginador;

            ////////////////////////
            // FILTRO DE BÚSQUEDA //
            ////////////////////////
            // Recuperamos el 'DbSet' completo
            var equipos = context.Equipos.Include (e => e.Marca).Include (e => e.Modelo);
            var ordenesReparacion = context.OrdenesReparacion.Include (x => x.Cliente).Include (x => x.Equipo)
                .Include (x => x.Tecnico).Include (x => x.TecnicoRxEquipo).Select (x => new OrdenReparacion {
                    Id = x.Id,
                        Equipo = equipos.FirstOrDefault (e => e.Id == x.EquipoId)
                }).ToList ();
            // Filtramos el resultado por el 'texto de búqueda'
            if (!string.IsNullOrEmpty (search)) {
                foreach (var item in search.Split (new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)) {
                    ordenesReparacion = ordenesReparacion.Where (x => x.Id.Equals (item)).ToList ();
                }
            }

            /////////////////////////////
            // ORDENACIÓN POR COLUMNAS //
            /////////////////////////////
            switch (order) {
                case "Id":
                    if (typeOrder.ToLower () == "desc")
                        ordenesReparacion = ordenesReparacion.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        ordenesReparacion = ordenesReparacion.OrderBy (x => x.Id).ToList ();
                    break;
                default:
                    if (typeOrder.ToLower () == "desc")
                        ordenesReparacion = ordenesReparacion.OrderByDescending (x => x.Id).ToList ();
                    else if (typeOrder.ToLower () == "asc")
                        ordenesReparacion = ordenesReparacion.OrderBy (x => x.Id).ToList ();
                    break;
            }

            ///////////////////////////
            // SISTEMA DE PAGINACIÓN //
            ///////////////////////////
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            // Número total de registros de la tabla OrdenesReparacion
            _TotalRegistros = ordenesReparacion.Count ();
            // Obtenemos la 'página de registros' de la tabla OrdenesReparacion
            ordenesReparacion = ordenesReparacion.Skip ((page - 1) * itemsPerPage)
                .Take (itemsPerPage)
                .ToList ();
            // Número total de páginas de la tabla OrdenesReparacion
            _TotalPaginas = (int) Math.Ceiling ((double) _TotalRegistros / itemsPerPage);

            // Instanciamos la 'Clase de paginación' y asignamos los nuevos valores
            _Paginador = new PaginadorGenerico<OrdenReparacion> () {
                ItemsPerPage = itemsPerPage,
                TotalItems = _TotalRegistros,
                TotalPages = _TotalPaginas,
                Page = page,
                Search = search,
                Order = order,
                TypeOrder = typeOrder,
                Result = ordenesReparacion
            };
            return Ok (_Paginador);
        }
        // GET TallerWebApi/OrdenesReparacion
        [HttpGet ("getAll")]
        public ActionResult GetAll () {
            var ordenesReparacion = context.OrdenesReparacion.Where (m => m.Activo == true).ToList ();
            return Ok (ordenesReparacion);
        }

        // GET: TallerWebApi/OrdenesReparacion/Id
        [HttpGet ("{id}", Name = "GetOrdenReparacion")]
        public IActionResult GetbyId (int id) {
            var ordenReparacion = context.OrdenesReparacion.FirstOrDefault (s => s.Id == id);
            if (ordenReparacion == null) {
                return NotFound ();
            }
            return Ok (ordenReparacion);
        }

        // POST TallerWebApi/OrdenesReparacion
        [HttpPost]
        public IActionResult POST ([FromBody] OrdenReparacionDto ordenReparacion) {
            if (ModelState.IsValid) {
                var o = new OrdenReparacion () {
                    Id = ordenReparacion.Id,
                    ClienteId = ordenReparacion.Cliente,
                    TecnicoRxEquipoId = ordenReparacion.TecnicoRxEquipo,
                    TecnicoId = ordenReparacion.Tecnico,
                    EquipoId = ordenReparacion.Equipo,
                    Defecto = ordenReparacion.Defecto,
                    Causa = ordenReparacion.Causa,
                    Accion = ordenReparacion.Accion,
                    DefectoCliente = ordenReparacion.DefectoCliente,
                    TiempoEmpleado = ordenReparacion.TiempoEmpleado,
                    FechaIngreso = ordenReparacion.FechaIngreso,
                    FechaFinalizado = ordenReparacion.FechaFinalizado,
                    FechaCerrado = ordenReparacion.FechaCerrado,
                    FechaPrometido = ordenReparacion.FechaPrometido,
                    Garantía = ordenReparacion.Garantía,
                    EstadoOrden = ordenReparacion.EstadoOrden,
                    TallerId = ordenReparacion.Taller,
                    PresupuestoId = ordenReparacion.Presupuesto,
                    InformeTecnico = ordenReparacion.InformeTecnico,
                    LugarReparacion = ordenReparacion.LugarReparacion,
                    Activo = ordenReparacion.Activo,
                };
                context.OrdenesReparacion.Add (o);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetOrdenReparacion", new { id = o.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/OrdenesReparacion/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] OrdenReparacionDto ordenReparacion, int id) {
            if (ordenReparacion.Id != id) {
                return BadRequest (ModelState);

            }
            var o = new OrdenReparacion () {
                Id = ordenReparacion.Id,
                ClienteId = ordenReparacion.Cliente,
                TecnicoRxEquipoId = ordenReparacion.TecnicoRxEquipo,
                TecnicoId = ordenReparacion.Tecnico,
                EquipoId = ordenReparacion.Equipo,
                Defecto = ordenReparacion.Defecto,
                Causa = ordenReparacion.Causa,
                Accion = ordenReparacion.Accion,
                DefectoCliente = ordenReparacion.DefectoCliente,
                TiempoEmpleado = ordenReparacion.TiempoEmpleado,
                FechaIngreso = ordenReparacion.FechaIngreso,
                FechaFinalizado = ordenReparacion.FechaFinalizado,
                FechaCerrado = ordenReparacion.FechaCerrado,
                FechaPrometido = ordenReparacion.FechaPrometido,
                Garantía = ordenReparacion.Garantía,
                EstadoOrden = ordenReparacion.EstadoOrden,
                TallerId = ordenReparacion.Taller,
                PresupuestoId = ordenReparacion.Presupuesto,
                InformeTecnico = ordenReparacion.InformeTecnico,
                LugarReparacion = ordenReparacion.LugarReparacion,
                Activo = ordenReparacion.Activo,
            };
            context.Entry (o).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/OrdenesReparacion/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var ordenReparacion = context.OrdenesReparacion.FirstOrDefault (s => s.Id == id);

            if (ordenReparacion.Id != id) {
                return NotFound ();
            }
            context.OrdenesReparacion.Remove (ordenReparacion);
            context.SaveChanges ();
            return Ok (ordenReparacion);
        }
    }
}