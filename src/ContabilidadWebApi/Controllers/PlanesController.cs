using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.ViewModels;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Services;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class PlanesController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public PlanesController(ContabilidadDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Devuelve un Listado de Planes En Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/Planes
        [HttpGet("{year}")]
        public IEnumerable<DetallePlanIGVM> GetPlanes(string year)
        {
            var detalle = new List<DetallePlanIGVM>();
            var detalles = _context.Set<DetallePlanGI>().Include(s => s.Concepto).Include(s => s.Plan).Where(s => s.Plan.Year == year);
            foreach (var item in detalles)
            {
                detalle.Add(new DetallePlanIGVM
                {
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
            return detalle;
        }

    }
}