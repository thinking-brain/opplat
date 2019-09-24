using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class GastosIndirectosController : ControllerBase
    {
        IConfiguration _config { get; set; }

        public GastosIndirectosController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Reporte de Gastos Indirectos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        [HttpGet("finanzas/GastosIndirectos/{años}/{mes}")]
        public List<GastosIndirectosVM> GastosIndirectos([FromRoute] string años, int mes)
        {
            //Periodo
            HttpClient clientPeriodo = new HttpClient();
            List<GenPeriodoVM> periodo = new List<GenPeriodoVM>();
            var url = _config.GetValue<string>("ContabilidadApi") + "/GenPeriodo";
            var resultPeriodo = clientPeriodo.GetAsync(url).Result;
            if (resultPeriodo.IsSuccessStatusCode)
            {
                periodo = resultPeriodo.Content.ReadAsAsync<List<GenPeriodoVM>>().Result;
            }

            //OptCuentaCentroSubPeriodo
            HttpClient clientOpt = new HttpClient();
            List<OptCuentaCentroSubPeriodoVM> opt = new List<OptCuentaCentroSubPeriodoVM>();
            var url2 = _config.GetValue<string>("ContabilidadApi") + "/OptCuentaCentroSubPeriodo";
            var resultOpt = clientOpt.GetAsync(url2).Result;
            if (resultOpt.IsSuccessStatusCode)
            {
                opt = resultOpt.Content.ReadAsAsync<List<OptCuentaCentroSubPeriodoVM>>().Result;
            }

            //CosSubelementogasto
            HttpClient clientSubElemento = new HttpClient();
            List<CosSubelementogastoVM> subElemento = new List<CosSubelementogastoVM>();
            var urlSubelement = _config.GetValue<string>("ContabilidadApi") + "/CosSubelementogastos";
            var resultSubElemento = clientSubElemento.GetAsync(urlSubelement).Result;
            if (resultSubElemento.IsSuccessStatusCode)
            {
                subElemento = resultSubElemento.Content.ReadAsAsync<List<CosSubelementogastoVM>>().Result;
            }


            int year;
            year = Convert.ToInt32(años);
            var data = new List<GastosIndirectosVM>();
            var periodoInicio = periodo.SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == 1);
            var periodoFin = periodo.SingleOrDefault(s => s.Inicio.Year == year && s.Inicio.Month == mes);
            foreach (var item in opt.Where(s => s.Idperiodo >= periodoInicio.Idperiodo && s.Idperiodo <= periodoFin.Idperiodo && s.Clavecuenta.Equals("731")).ToList().GroupBy(s => s.Idsub))
            {
                var sub = subElemento.SingleOrDefault(s => s.Idsubelemento == item.Key);
                data.Add(new GastosIndirectosVM { DescripcionSubElemento = sub.Descripcion, Valor = item.Sum(s => s.Debito) });
            }
            return data;
        }
    }
}