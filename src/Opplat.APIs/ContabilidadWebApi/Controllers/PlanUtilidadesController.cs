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
    public class PlanUtilidadesController : ControllerBase {
        private readonly ContabilidadDbContext _context;

        public PlanUtilidadesController (ContabilidadDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Devuelve un Listado de Planes de Ingresos En Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/PlanIngresos
        [HttpGet ("{year}")]
        public IEnumerable<DetallePlanIGVM> GetPlanUtilidades (string year) {
            var ConceptoUtilidades = new List<string> () {

                "Pago a cargo de la utilidad",
                "Utilidad despues de pagos de anticipos",
                "Reserva de contingencia del 2% al 10%",
                "Utilidad libre despues de la reserva",
                "Reserva de contingencia 30%",
                "Utilidad despues de la reserva del 30%"
            };
            var detalle = new List<DetallePlanIGVM> ();
            var detalles = _context.Set<DetallePlanGI> ().Include (s => s.Concepto).Include (s => s.Plan).Where (s => s.Plan.Year == year);

            foreach (var item in detalles.OrderBy (s => s.ConceptoId)) {
                if (ConceptoUtilidades.ToList ().Contains (item.Concepto.Concepto.ToString ())) {
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