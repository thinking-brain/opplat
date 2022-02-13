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
    public class PlanEgresosController : ControllerBase {
        private readonly ContabilidadDbContext _context;

        public PlanEgresosController (ContabilidadDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Devuelve un Listado de Planes de Ingresos En Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/PlanIngresos
        [HttpGet ("{year}")]
        public IEnumerable<DetallePlanIGVM> GetPlanEngresos (string year) {
            var ConceptoEgresos = new List<string> () {

                "Impuestos por las ventas",
                "Costo de ventas",
                "Gastos generales de Administración",
                "Gastos financieros en MN",
                "Gastos financieros en MLC",
                "Gastos por perdida",
                "Otros impuestos tasas y contribuciones "
            };
            var detalle = new List<DetallePlanIGVM> ();
            var detalles = _context.Set<DetallePlanGI> ().Include (s => s.Concepto).Include (s => s.Plan).Where (s => s.Plan.Year == year);

            foreach (var item in detalles.OrderBy (s => s.ConceptoId)) {
                if (ConceptoEgresos.ToList ().Contains (item.Concepto.Concepto.ToString ())) {

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