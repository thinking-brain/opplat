using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using op_costos_api.Data;
using op_costos_api.Helper;
using op_costos_api.Models;
using op_costos_api.VersatModels;
using op_costos_api.ViewModels;

namespace op_costos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CostosController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly CostReportHelper helper;


        public CostosController(ApiDbContext context, VersatDbContext vcontext)
        {
            _context = context;
            _vcontext = vcontext;
            helper = new CostReportHelper(_vcontext, _context);
        }
        /// <summary>
        /// Devuelve Lista de Años
        /// </summary>
        /// <returns></returns>
        private static string[] getYears()
        {
            return new[] { "2018", "2019", "2020", "2021", "2022" };
        }

        /// <summary>
        /// Selector de Areas para Vistas
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/SelectAreas/")]
        public List<dynamic> SelectAreas()
        {
            var area = _context.Set<CentroCostoArea>().GroupBy(s => s.CentroCostoId).Select(s => new { Id = s.First().Id, Detalles = s.First().Detalles + " - " + s.First().CentroCostoId });
            var lista = new List<dynamic>();
            foreach (var item in area)
            {
                lista.Add(new { Id = item.Id, Detalles = item.Detalles });
            }
            return lista.ToList();
        }
        /// <summary>
        /// Selector de Meses para Vistas
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/SelectMeses/")]
        public List<dynamic> SelectMeses()
        {
            var Meses = new List<dynamic>() {
                new { Id = 1, Nombre = Mes.Enero.ToString() },
                new { Id = 2, Nombre = Mes.Feberero.ToString() },
                new { Id = 3, Nombre = Mes.Marzo.ToString() },
                new { Id = 4, Nombre = Mes.Abril.ToString() },
                new { Id = 5, Nombre = Mes.Mayo.ToString() },
                new { Id = 6, Nombre = Mes.Junio.ToString() },
                new { Id = 7, Nombre = Mes.Julio.ToString() },
                new { Id = 8, Nombre= Mes.Agosto.ToString() },
                new { Id = 9, Nombre= Mes.Septiembre.ToString() },
                new { Id = 10, Nombre = Mes.Octubre.ToString() },
                new { Id = 11, Nombre= Mes.Noviembre.ToString() },
                new { Id = 12, Nombre = Mes.Diciembre.ToString() }
            };
            return Meses.ToList();
        }
        /// <summary>
        /// Selector de Años para Vistas
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/SelectAños/")]
        public List<dynamic> SelectAños()
        {
            var Años = new List<dynamic>();
            foreach (var item in _context.Set<Plan>())
            {
                Años.Add(new
                {
                    Año = item.Fecha.Year
                });

            }
            return Años.Distinct().ToList();
        }
        /// <summary>
        /// Selector de Cuentas para Vistas
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/SelectCuentas/")]
        public List<dynamic> SelectCuentas()
        {
            var cuentas = new List<dynamic>();
            foreach (var item in _vcontext.ConCuenta.ToList())
            {
                cuentas.Add(new { Numero = item.Clave.ToString() });
            }
            return cuentas;
        }

        /// <summary>
        /// Costos Agrupados por Grupos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <param name="areas"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        [HttpGet("api/CostoGroup/{areas}/{cuenta}/{años}/{meses}")]
        public CostosGroupApiVM CostoGroup([FromRoute] string años, int meses, int areas, string cuenta)
        {
            int year;
            CentroCostoArea centroArea;
            List<CostosViewModel> Costo;
            var report = new CostReportHelper(_vcontext, _context);

            report.ObtenerCosto(años, meses, areas, cuenta, out year, out centroArea, out Costo);

            var grupos = Costo.Where(s => s.Grupo != "").GroupBy(s => s.Grupo).ToList();

            Dictionary<string, Dictionary<string, CostosViewModel>> valores;
            Dictionary<string, List<decimal>> totales;
            List<decimal> general;

            report.FormatearSalidaGeneral(Costo, grupos, out valores, out totales, out general);

            var datos = new CostosGroupApiVM
            {
                Titulo = centroArea.CentroCostoId + "  " + centroArea.Area.Nombre,
                Fecha = ((Mes)meses).ToString() + " " + year,
                Parciales = totales,
                Valores = valores,
                General = general
            };

            return datos;
        }

        /// <summary>
        /// Costos Agrupados por Grupo Modo Avanzado
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <param name="areas"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        [HttpGet("api/CostoGroupAdvanced/{años}/{meses}/{areas}/{cuenta}")]
        public CostoGroupAdvancedApiVM CostoGroupAdvanced([FromRoute] string años, int meses, int areas, string cuenta)
        {
            int year;
            CentroCostoArea centroArea;
            List<CostosViewModel> Costo;
            var report = new CostReportHelper(_vcontext, _context);

            report.ObtenerCosto(años, meses, areas, cuenta, out year, out centroArea, out Costo);

            Dictionary<string, List<CostosViewModel>> valores = new Dictionary<string, List<CostosViewModel>>();
            Dictionary<string, List<decimal>> parciales = new Dictionary<string, List<decimal>>();
            List<decimal> general = new List<decimal>();
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);
            foreach (var item in Costo)
            {
                var llave = item.Elementos;
                if (!valores.ContainsKey(llave))
                {
                    valores.Add(llave, new List<CostosViewModel>());
                    parciales.Add(llave, new List<decimal>());
                    parciales[llave].Add(0);
                    parciales[llave].Add(0);
                    parciales[llave].Add(0);
                    parciales[llave].Add(0);
                    parciales[llave].Add(0);
                    parciales[llave].Add(0);

                }
                valores[llave].Add(item);
                parciales[llave][0] += item.PlanMes;
                parciales[llave][1] += item.RealMes;
                parciales[llave][2] += item.PorcEjec;
                parciales[llave][3] += item.PlanAcumulado;
                parciales[llave][4] += item.RealAcumulado;
                parciales[llave][5] += item.PorcRealEjec;

                general[0] += item.PlanMes;
                general[1] += item.RealMes;
                general[2] += item.PorcEjec;
                general[3] += item.PlanAcumulado;
                general[4] += item.RealAcumulado;
                general[5] += item.PorcRealEjec;

            }

            var datos = new CostoGroupAdvancedApiVM
            {
                Titulo = centroArea.CentroCostoId + "  " + centroArea.Area.Nombre,
                Fecha = ((Mes)meses).ToString() + " " + year,
                Parciales = parciales,
                Valores = valores,
                General = general
            };

            return datos;
        }
        /// <summary>
        /// Reporte de Costo por Columnas
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <param name="areas"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        [HttpGet("api/CostColumnReport/{años}/{meses}/{areas}/{cuenta}")]
        public CostColumnReportApiVM CostColumnReport([FromRoute] string años, int meses, int areas, string cuenta)
        {

            int year;
            CentroCostoArea centroArea;
            List<CostosViewModel> Costo;
            var report = new CostReportHelper(_vcontext, _context);

            Dictionary<string, Dictionary<string, CostosViewModel>> valores;
            Dictionary<string, List<decimal>> totales;
            List<decimal> general;

            report.ObtenerCosto(años, meses, areas, cuenta, out year, out centroArea, out Costo);

            var grupos = Costo.Where(s => s.Grupo != "").GroupBy(s => s.Grupo).ToList();

            report.FormatearSalidaGeneral(Costo, grupos, out valores, out totales, out general);
            string[] head = { "PlanMes", "RealMes", "%Ejec", "PlanAcum", "RealAcum", "%EjecAcum" };

            var datos = new CostColumnReportApiVM
            {
                Titulo = centroArea.CentroCostoId + "  " + centroArea.Area.Nombre,
                Fecha = ((Mes)meses).ToString() + " " + year,
                Parciales = totales,
                Valores = valores,
                General = general,
                Headers = head

            };

            return datos;
        }
        /// <summary>
        /// Costos Todos
        /// </summary>
        /// <param name="cuenta"></param>
        /// <param name="años"></param>
        /// <param name="areas"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        // GET api/values/5
        [HttpGet("api/Costo/{areas}/{cuenta}/{años}/{meses}")]
        public IEnumerable<CostosViewModel> Costo([FromRoute] string cuenta, string años, int areas, int meses)
        {
            int year;
            CentroCostoArea centroArea;
            List<CostosViewModel> Costo;
            var report = new CostReportHelper(_vcontext, _context);
            report.ObtenerCosto(años, meses, areas, cuenta, out year, out centroArea, out Costo);

            return Costo.ToList();
        }
    }
}
