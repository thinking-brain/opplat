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

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class RazonesFinancierasController : ControllerBase
    {
        ObtenerValuesEnVariablesEstadoFinanciero _obtenerVariables { get; set; }
        ObtenerPlanGI _obtenetPlan { get; set; }
        IConfiguration _config { get; set; }

        public RazonesFinancierasController(ObtenerValuesEnVariablesEstadoFinanciero obtenerVariables, ObtenerPlanGI obtenerPlan, IConfiguration config)
        {
            _config = config;
            _obtenerVariables = obtenerVariables;
            _obtenetPlan = obtenerPlan;
        }




        /// <summary>
        /// Devuelve el valor de la Razon (Solvencia Financiera)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("solvenciaFinanciera/{años}/{meses}")]
        public decimal solvenciaFinanciera(int años, int meses)
        {
            decimal ingresos = _obtenetPlan.ObtenerTotalIngresos(años, meses);
            decimal egresos = _obtenetPlan.ObtenerTotalEgresos(años, meses);
            decimal resultado = egresos != 0 ? ingresos / egresos : 0;
            return Math.Round(resultado, 2);
        }

        /// <summary>
        /// Devuelve el valor de la Razon (Capital  de trabajo)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("capitalDeTrabajo/{años}/{meses}")]
        public decimal capitalDeTrabajo(int años, int meses)
        {
            decimal ac = _obtenerVariables.activoCirculante(años, meses);
            decimal pc = _obtenerVariables.pasivoCirculante(años, meses);
            decimal resultado = ac - pc;
            return resultado;
        }

        /// <summary>
        /// Devuelve el valor de la Razon (Indice de liquidez Genaral)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("indiceDeLiquidezGneral/{años}/{meses}")]
        public decimal indiceDeLiquidezGneral(int años, int meses)
        {
            decimal ac = _obtenerVariables.activoCirculante(años, meses);
            decimal pc = _obtenerVariables.pasivoCirculante(años, meses);
            decimal resultado = pc != 0 ? ac / pc : 0;
            return resultado;
        }

        /// <summary>
        /// Devuelve el valor de la Razon (Liquidez de Tesorería)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("liquidezDeTesoreria/{años}/{meses}")]
        public decimal liquidezDeTesoreria(int años, int meses)
        {
            //ACTIVOS LIQUIDOS NO ESTAN
            decimal al = _obtenerVariables.activoCirculante(años, meses);
            decimal pc = _obtenerVariables.pasivoCirculante(años, meses);
            decimal resultado = pc != 0 ? al / pc : 0;
            return resultado;
        }


        /// <summary>
        /// Devuelve el valor de la Razon (Indice de deuda o Razon de Endeudamiento)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("indiceDeDeudaORazonDeEndeudamiento/{años}/{meses}")]
        public decimal indiceDeDeudaORazonDeEndeudamiento(int años, int meses)
        {
            decimal pc = _obtenerVariables.pasivoCirculante(años, meses);
            decimal plp = _obtenerVariables.pasivosALargoPlazo(años, meses);
            decimal pd = _obtenerVariables.pasivosDiferidos(años, meses);
            decimal op = _obtenerVariables.otrosPasivos(años, meses);
            decimal tp = pc + plp + pd + op;

            decimal ac = _obtenerVariables.activoCirculante(años, meses);
            decimal ad = _obtenerVariables.activosDiferidos(años, meses);
            decimal af = _obtenerVariables.activosFijos(años, meses);
            decimal alp = _obtenerVariables.activosLargoPlazo(años, meses);
            decimal oa = _obtenerVariables.otrosActivos(años, meses);
            decimal ta = ac + ad + af + alp + oa;

            decimal resultado = ta != 0 ? tp / ta : 0;
            return resultado;
        }


        /// <summary>
        /// Devuelve el valor de la Razon (Indice de disponibilidad)
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("indiceDeDisponibilidad/{años}/{meses}")]
        public decimal indiceDeDisponibilidad(int años, int meses)
        {
            decimal ec = _obtenerVariables.efectivoEnCaja(años, meses);
            decimal eb = _obtenerVariables.efectivoEnBanco(años, meses);
            decimal ac = _obtenerVariables.activoCirculante(años, meses);

            decimal resultado = ac != 0 ? (ec + eb) / ac : 0;
            return resultado;
        }



    }
}