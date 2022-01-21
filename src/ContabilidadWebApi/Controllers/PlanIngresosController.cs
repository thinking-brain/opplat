using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Services;
using ContabilidadWebApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Controllers {
    [Route ("contabilidad/[controller]")]
    [ApiController]
    public class PlanIngresosController : ControllerBase {
        private readonly ContabilidadDbContext _context;

        public PlanIngresosController (ContabilidadDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Devuelve un Listado de Planes de Ingresos
        /// </summary>
        /// <returns></returns>
        // GET: api/PlanIngresosAll
        [HttpGet]
        public IEnumerable<PlanGI> GetPlanIngresosAll () {
            var planes = new List<PlanGI> ();

            foreach (var item in _context.Set<DetallePlanGI> ().Include (s => s.Concepto)) {
                var concepto = DatosPlanGI.Datos ().SingleOrDefault (s => s.Dato.Equals (item.Concepto.Concepto)).Valor;
                if (concepto.Equals ("Ingresos")) {
                    var plan = _context.Set<PlanGI> ().SingleOrDefault (s => s.Id == item.Id);
                    planes.Add (plan);
                }

            }
            return planes.ToList ();
        }

        /// <summary>
        /// Devuelve un Listado de Planes de Ingresos En Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/PlanIngresos
        [HttpGet ("{year}")]
        public IEnumerable<DetallePlanIGVM> GetPlanIngresos (string year) {
            var ConceptoIngresos = new List<string> () {

                "Ventas por servicios en moneda nacional",
                "Ventas por servicios en CUC",
                "Ingresos financieros"
            };
            var detalle = new List<DetallePlanIGVM> ();
            var detalles = _context.Set<DetallePlanGI> ().Include (s => s.Concepto).Include (s => s.Plan).Where (s => s.Plan.Year == year);

            foreach (var item in detalles.OrderBy (s => s.ConceptoId)) {
                if (ConceptoIngresos.ToList ().Contains (item.Concepto.Concepto.ToString ())) {
                    // var concepto = DatosPlanGI.Datos ().SingleOrDefault (s => s.Dato.Equals (item.Concepto.Concepto)) != null ? DatosPlanGI.Datos ().SingleOrDefault (s => s.Dato.Equals (item.Concepto.Concepto)).Tipo : "Otros";

                    detalle.Add (new DetallePlanIGVM {
                        Concepto = item.Concepto.Concepto,
                            Enero = item.Enero,
                            Febrero = item.Febrero,
                            Marzo = item.Marzo,
                            Abril = item.Abril,
                            Mayo = item.Mayo,
                            Junio = item.Junio,
                            Julio = item.Julio,
                            Agosto = item.Agosto,
                            Septiembre = item.Septiembre,
                            Octubre = item.Octubre,
                            Noviembre = item.Noviembre,
                            Diciembre = item.Diciembre

                    });

                }
            }
            return detalle;
        }

    }
}