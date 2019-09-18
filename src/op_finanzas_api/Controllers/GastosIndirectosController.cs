using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using op_finanzas_api.Models;
using op_finanzas_api.ViewModels;
using System.Net.Http;

namespace op_finanzas_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastosIndirectosController : ControllerBase
    {
        /// <summary>
        /// Reporte de Gastos Indirectos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [HttpGet("api/GastosIndirectos/{años}/{mes}")]
        public List<GastosIndirectosVM> GastosIndirectos([FromRoute] string años, int mes)
        {
            //Periodo
            HttpClient clientPeriodo = new HttpClient();
            List<GenPeriodoVM> periodo = new List<GenPeriodoVM>();
            var resultPeriodo = clientPeriodo.GetAsync( "http://localhost:5050/api/GenPeriodo").Result;
            if (resultPeriodo.IsSuccessStatusCode)
            {
                periodo = resultPeriodo.Content.ReadAsAsync<List<GenPeriodoVM>>().Result;
            }

            //OptCuentaCentroSubPeriodo
            HttpClient clientOpt = new HttpClient();
            List<OptCuentaCentroSubPeriodoVM> opt = new List<OptCuentaCentroSubPeriodoVM>();
            var resultOpt = clientOpt.GetAsync( "http://localhost:5050/api/OptCuentaCentroSubPeriodo").Result;
            if (resultOpt.IsSuccessStatusCode)
            {
                opt = resultOpt.Content.ReadAsAsync<List<OptCuentaCentroSubPeriodoVM>>().Result;
            }
          
           //CosSubelementogasto
            HttpClient clientSubElemento = new HttpClient();
            List<CosSubelementogastoVM> subElemento = new List<CosSubelementogastoVM>();
            var resultSubElemento = clientSubElemento.GetAsync( "http://localhost:5050/api/CosSubelementogastos").Result;
            if (resultSubElemento.IsSuccessStatusCode)
            {
                subElemento = resultSubElemento.Content.ReadAsAsync<List<CosSubelementogastoVM>>().Result;
            }
          

            int year;
            year = Convert.ToInt32(años);
            var data = new List<GastosIndirectosVM>();
            var periodoInicio = periodo.SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == 1);
            var periodoFin = periodo.SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == mes);
            foreach (var item in opt.Where(s=>s.Idperiodo >= periodoInicio.Idperiodo && s.Idperiodo <= periodoFin.Idperiodo && s.Clavecuenta.Equals("731")).ToList().GroupBy(s=>s.Idsub))
            {
                var sub = subElemento.SingleOrDefault(s => s.Idsubelemento == item.Key);
                data.Add(new GastosIndirectosVM { DescripcionSubElemento = sub.Descripcion, Valor = item.Sum(s => s.Debito) });
            }
            return data;
        }
    }
}