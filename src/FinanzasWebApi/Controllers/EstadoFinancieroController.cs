using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.Helper;
using FinanzasWebApi.ViewModels;
using Microsoft.Extensions.Configuration;
using FinanzasWebApi.Helper.EstadoFinanciero;
using FinanzasWebApi.Models;
using Newtonsoft.Json;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class EstadoFinancieroController : ControllerBase
    {
        GetEstadoFinanciero _obtenerEstadoFinanciero { get; set; }
        GetEF _obtenerEF { get; set; }
        IConfiguration _config { get; set; }
        FinanzasDbContext _context { get; set; }

        public EstadoFinancieroController(FinanzasDbContext context, GetEstadoFinanciero obtenerEstadoFinaciero, GetEF obtenerEF, IConfiguration config)
        {
            _context = context;
            _config = config;
            _obtenerEstadoFinanciero = obtenerEstadoFinaciero;
            _obtenerEF = obtenerEF;
        }



        [HttpGet("estadoFinanciero5920/{años}/{meses}")]
        public List<EstadoFinancieroJsVM> EstadoFinancieros5920(int años, int meses)
        {
            string efe = "5920";
            var resultado = _obtenerEF.estadoFinanciero5920(años, meses);
            return resultado;
        }

        [HttpGet("estadoFinanciero5920Report/{años}/{meses}")]
        public List<EstadoFinancieroJsVM> EstadoFinancieros5920Report(int años, int meses)
        {
            var resultado = _obtenerEF.estadoFinanciero5920Report(años, meses);
            return resultado;
        }

        [HttpGet("estadoFinanciero5921Report/{años}/{meses}")]
        public List<EstadoFinancieroJsVM> EstadoFinancieros5921(int años, int meses)
        {
            var resultado = _obtenerEF.estadoFinanciero5921(años, meses);
            return resultado;
        }

        [HttpGet("configurarReporte/{tipo_plan}")]
        public ActionResult configurarReporte(string tipo_plan)
        {
            var urlitems = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{tipo_plan}.json";
            var urlselection = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{tipo_plan}Hoja.json";
            var plan = System.IO.File.ReadAllText(urlitems);
            var planconfig = System.IO.File.ReadAllText(urlselection);
            dynamic obj = new
            {
                items = JsonConvert.DeserializeObject<List<dynamic>>(plan),
                selection = JsonConvert.DeserializeObject<List<dynamic>>(planconfig),
            };
            return Ok(obj);
        }
        [HttpPost("configurarReporte")]
        public ActionResult configurarReportePost(ConfiguradorPlanViewModel viewModel)
        {
            var url = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{viewModel.TipoPlan}Hoja.json";
            string json = JsonConvert.SerializeObject(viewModel.Items);
            System.IO.File.WriteAllText(url, json);
            var result = JsonConvert.DeserializeObject<List<dynamic>>(json);
            return Ok(result);
        }


        /// <summary>
        /// Devuelve el valor de la Razon (Solvencia Financiera)
        /// </summary>
        /// <returns></returns>
        [HttpGet("estado/")]
        public List<string> Estado()
        {
            var data = new List<string>();
            foreach (var item in GetEstadoFinancieroDatos.EstadoFinancieroDatos().Where(s => s.EF == "5920").ToList())
            {
                data.Add("decimal " + item.UB.ToString() + " = _obtenerEstadoFinanciero.EstadoFinancieroValue(años, meses, efe, " + '"' + item.UB.ToString() + '"' + ");");

            }

            return data;
        }


        /// <summary>
        /// Actualizar la Caché de los Esatdos Financieros
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("cacheUpdate/{year}")]
        public IActionResult CacheUpdate(int year)
        {
            var result = UpdateCache(year);
            if (!result)
            {
                return BadRequest("Error actualizando la cache del estado fianciero.");
            }
            return Ok("Actualizacion realizada correctamente.");
        }

        private bool UpdateCache(int year)
        {
            var datos = GetEstadoFinancieroDatos.EstadoFinancieroDatos().ToList();
            try
            {
                foreach (var item in datos)
                {
                    var cuentas = item.Valor.Split(",");
                    string concepto = item.Dato.ToString();
                    var grupo = item.Grupo.ToString();
                    var efe = item.EF.ToString();

                    for (int i = 1; i < 13; i++)
                    {
                        decimal saldo = 0;
                        foreach (var cta in cuentas)
                        {
                            var val = GetMovimientoDeCuentaPeriodo.Get(year, i, cta, _config);
                            saldo = saldo + val;
                        }
                        var data = _context.Set<CacheEstadoFinanciero>().SingleOrDefault(c => c.Concepto == concepto && c.Mes == i && c.Year == year);
                        if (data == null)
                        {
                            _context.Add(new CacheEstadoFinanciero
                            {
                                Grupo = grupo,
                                EFE = efe,
                                Concepto = concepto,
                                FechaActualizado = DateTime.Now,
                                Mes = i,
                                Year = year,
                                Saldo = saldo,
                                PlanAnual = 0,
                                Apertura = 0
                            });
                        }
                        else
                        {
                            data.Saldo = saldo;
                            data.PlanAnual = 0;
                            data.FechaActualizado = DateTime.Now;
                        }

                        _context.SaveChanges();
                    }

                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}