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
    public class VariablesFinancierasController : ControllerBase
    {
        ObtenerValuesEnVariablesEstadoFinanciero _obtenerVariables { get; set; }
        IConfiguration _config { get; set; }

        public VariablesFinancierasController(ObtenerValuesEnVariablesEstadoFinanciero obtenerVariables, IConfiguration config)
        {
            _config = config;
            _obtenerVariables = obtenerVariables;
        }




        /// <summary>
        /// Devuelve el valor Total del Inventario
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("totalInventario/{años}/{meses}")]
        public decimal VariableTI(int años, int meses)
        {
            decimal valor = _obtenerVariables.InventarioTotal(años, meses);
            return valor;
        }


        /// <summary>
        /// Devuelve el Valor de los Activos Circulantes
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("activoCirculante/{años}/{meses}")]
        public decimal VariableAC(int años, int meses)
        {
            decimal valor = _obtenerVariables.activoCirculante(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve los Activos a Largo Plazo
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("activosLargoPlazo/{años}/{meses}")]
        public decimal VariableALP(int años, int meses)
        {
            decimal valor = _obtenerVariables.activosLargoPlazo(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve el valor de los Activos Fijos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("activosFijos/{años}/{meses}")]
        public decimal VariableAFT(int años, int meses)
        {
            decimal valor = _obtenerVariables.activosFijos(años, meses);
            return valor;

        }

        /// <summary>
        /// Devuelve el valor de los Activos Diferidos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("activosDiferidos/{años}/{meses}")]
        public decimal VariableAFD(int años, int meses)
        {
            decimal valor = _obtenerVariables.activosDiferidos(años, meses);
            return valor;

        }

        /// <summary>
        /// Devuelve el valor de Otros Activos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("otrosActivos/{años}/{meses}")]
        public decimal VariableOA(int años, int meses)
        {
            decimal valor = _obtenerVariables.otrosActivos(años, meses);
            return valor;
        }


        /// <summary>
        /// Devuelve el valor del Total de Activos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("totalDeActivos/{años}/{meses}")]
        public decimal VariableTA(int años, int meses)
        {
            decimal ac = _obtenerVariables.activoCirculante(años, meses);
            decimal alp = _obtenerVariables.activosLargoPlazo(años, meses);
            decimal af = _obtenerVariables.activosFijos(años, meses);
            decimal ad = _obtenerVariables.activosDiferidos(años, meses);
            decimal oa = _obtenerVariables.otrosActivos(años, meses);
            decimal valor = ac + alp + af + ad + oa;
            return valor;
        }

        /// <summary>
        /// Devuelve el valor los Pasivos Circulantes
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("pasivosCirculantes/{años}/{meses}")]
        public decimal VariablePC(int años, int meses)
        {
            decimal valor = _obtenerVariables.pasivoCirculante(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve el valor los Pasivos A Largo Plazo
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("pasivosLargoPlazo/{años}/{meses}")]
        public decimal VariablePLP(int años, int meses)
        {
            decimal valor = _obtenerVariables.pasivosALargoPlazo(años, meses);
            return valor;
        }
        /// <summary>
        /// Devuelve el valor los Pasivos Diferidos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("pasivosDiferidos/{años}/{meses}")]
        public decimal VariablePD(int años, int meses)
        {
            decimal valor = _obtenerVariables.pasivosDiferidos(años, meses);
            return valor;
        }
        /// <summary>
        /// Devuelve el valor los Otros Pasivos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("otrosPasivos/{años}/{meses}")]
        public decimal VariableOP(int años, int meses)
        {
            decimal valor = _obtenerVariables.otrosPasivos(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve el valor los Totales de Pasivos
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("totalPasivos/{años}/{meses}")]
        public decimal VariableTP(int años, int meses)
        {
            decimal pc = _obtenerVariables.pasivoCirculante(años, meses);
            decimal plp = _obtenerVariables.pasivosALargoPlazo(años, meses);
            decimal pd = _obtenerVariables.pasivosDiferidos(años, meses);
            decimal op = _obtenerVariables.otrosPasivos(años, meses);
            decimal valor = pc + plp + pd + op;
            return valor;
        }
        /// <summary>
        /// Devuelve el valor de Efectivo en Caja
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("efectivoEnCaja/{años}/{meses}")]
        public decimal VariableEC(int años, int meses)
        {
            decimal valor = _obtenerVariables.efectivoEnCaja(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve el valor de Efectivo en Banco
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("efectivoEnBanco/{años}/{meses}")]
        public decimal VariableEB(int años, int meses)
        {
            decimal valor = _obtenerVariables.efectivoEnBanco(años, meses);
            return valor;
        }

        /// <summary>
        /// Devuelve el valor del Patrimonio Neto Total
        /// </summary>
        /// <param name="años"></param>
        /// <param name="meses"></param>
        /// <returns></returns>
        [HttpGet("totalPatrimonioNeto/{años}/{meses}")]
        public decimal VariableTPN(int años, int meses)
        {
            decimal valor = _obtenerVariables.totalDePatrimonioNeto(años, meses);
            return valor;
        }

    }
}