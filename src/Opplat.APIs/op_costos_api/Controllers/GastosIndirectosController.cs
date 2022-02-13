using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using op_costos_api.Models;
using op_costos_api.ViewModels;
using op_costos_api.VersatModels;

namespace op_costos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastosIndirectosController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;


        public GastosIndirectosController(ApiDbContext context, VersatDbContext vcontext)
        {
            _context = context;
            _vcontext = vcontext;
        }
        /// <summary>
        /// Reporte de Gastos Indirectos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [HttpGet("api/GastosIndirectos/{años}/{mes}")]
        public List<Cos_GastosIndirectos> GastosIndirectos([FromRoute] string años, int mes)
        {
            int year;
            year = Convert.ToInt32(años);
            var data = new List<Cos_GastosIndirectos>();
            var periodoInicio = _vcontext.Set<GenPeriodo>().SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == 1);
            var periodoFin = _vcontext.Set<GenPeriodo>().SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == mes);
            foreach (var item in _vcontext.Set<OptCuentaCentroSubPeriodo>().Where(s=>s.Idperiodo >= periodoInicio.Idperiodo && s.Idperiodo <= periodoFin.Idperiodo && s.Clavecuenta.Equals("731")).ToList().GroupBy(s=>s.Idsub))
            {
                var sub = _vcontext.CosSubelementogasto.SingleOrDefault(s => s.Idsubelemento == item.Key);
                data.Add(new Cos_GastosIndirectos { DescripcionSubElemento = sub.Descripcion, Valor = item.Sum(s => s.Debito) });
            }
            return data;
        }
    }
}