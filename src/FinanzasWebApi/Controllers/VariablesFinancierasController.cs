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
            decimal materiasPrimasYMateriales = _obtenerVariables.Valor("Materias Primas y Materiales", años, meses);
            decimal combustiblesYLubricantes = _obtenerVariables.Valor("Combustibles y Lubricantes", años, meses);
            decimal partesYPiezasDeRespuesto = _obtenerVariables.Valor("Partes y Piezas de Respuesto", años, meses);
            decimal envasesYEmbalajes = _obtenerVariables.Valor("Envases y Embalajes", años, meses);
            decimal utilesHerramientasYOtros = _obtenerVariables.Valor("Utiles, Herramientas y Otros", años, meses);
            decimal produccionTerminada = _obtenerVariables.Valor("Producción Terminada", años, meses);
            decimal mercanciasParaLaVenta = _obtenerVariables.Valor("Mercancias para la Venta", años, meses);
            decimal medicamentos = _obtenerVariables.Valor("Medicamentos", años, meses);
            decimal baseMaterialDeEstudio = _obtenerVariables.Valor("Base Material de Estudio", años, meses);
            decimal vestuarioYLenceria = _obtenerVariables.Valor("Vestuario y Lencería", años, meses);
            decimal alimentos = _obtenerVariables.Valor("Alimentos", años, meses);
            decimal inventarioDeMercanciasDeImportacion = _obtenerVariables.Valor("Inventario de Mercancías de Importación", años, meses);
            decimal inventarioDeMercanciasDeExportación = _obtenerVariables.Valor("Inventario de Mercancias de Exportación", años, meses);
            decimal produccionesParaInsumoOAutoconsumo = _obtenerVariables.Valor("Producciones para Insumo o Autoconsumo", años, meses);
            decimal otrosInventarios = _obtenerVariables.Valor("Otros Inventarios", años, meses);
            decimal inventariosOciosos = _obtenerVariables.Valor("Inventarios Ociosos", años, meses);
            decimal inventariosDeLentoMovimiento = _obtenerVariables.Valor("Inventarios de lento Movimiento", años, meses);
            decimal produccionEnProceso = _obtenerVariables.Valor("Producción en Proceso", años, meses);
            decimal produccionPropiaParaInsumoOAutoconsumo = _obtenerVariables.Valor("Produccion Propia para Insumo o Autoconsumo", años, meses);
            decimal reparacionesCapitalesConMediosPropios = _obtenerVariables.Valor("Reparaciones Capitales con Medios Propios", años, meses);
            decimal inversionesConMediosPropios = _obtenerVariables.Valor("Inversiones con Medios Propios", años, meses);


            decimal desgasteDeUtilesYHerramientas = _obtenerVariables.Valor("Desgaste de Utiles y Herramientas", años, meses);
            decimal menosDesgasteDeBaseMaterialDeEstudio = _obtenerVariables.Valor("Menos: Desgaste de Base Material de Estudio", años, meses);
            decimal menosDesgasteDeVestuarioYLenceria = _obtenerVariables.Valor("Menos: Desgaste de Vestuario y Lenceria", años, meses);


            decimal resultado =
              (
              materiasPrimasYMateriales
            + combustiblesYLubricantes
            + partesYPiezasDeRespuesto
            + envasesYEmbalajes
            + utilesHerramientasYOtros
            + produccionTerminada
            + mercanciasParaLaVenta
            + medicamentos
            + baseMaterialDeEstudio
            + vestuarioYLenceria
            + alimentos
            + inventarioDeMercanciasDeImportacion
            + inventarioDeMercanciasDeExportación
            + produccionesParaInsumoOAutoconsumo
            + otrosInventarios
            + inventariosOciosos
            + inventariosDeLentoMovimiento
            + produccionEnProceso
            + produccionPropiaParaInsumoOAutoconsumo
            + reparacionesCapitalesConMediosPropios
            + inversionesConMediosPropios
            )
            - (desgasteDeUtilesYHerramientas + menosDesgasteDeBaseMaterialDeEstudio - menosDesgasteDeVestuarioYLenceria)
            ;
            return resultado;
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
            decimal cuentaEnParticipacion = _obtenerVariables.Valor("Cuenta en Participación", años, meses);
            decimal cuentasPorCobrarACortoPlazo = _obtenerVariables.Valor("Cuentas por Cobrar a Corto Plazo", años, meses);
            decimal efectivoEnCaja = _obtenerVariables.Valor("Efectivo en Caja", años, meses);
            decimal efectivoEnBancoYEnOtrasInstituciones = _obtenerVariables.Valor("Efectivo en Banco y en otras instituciones", años, meses);
            decimal inversionesACortoPlazoOTemporales = _obtenerVariables.Valor("Inversiones a Corto Plazo o Temporales", años, meses);
            decimal efectosPorCobrarACortoPlazo = _obtenerVariables.Valor("Efectos por Cobrar a Corto Plazo", años, meses);
            decimal pagosPorCuentasDeTerceros = _obtenerVariables.Valor("Pagos por Cuentas de Terceros", años, meses);
            decimal creditosDocumentarios = _obtenerVariables.Valor("Créditos Documentarios", años, meses);
            decimal totalInventario = _obtenerVariables.InventarioTotal(años, meses);

            decimal efectosPorCobrarDescontados = _obtenerVariables.Valor("Efectos por Cobrar Descontados", años, meses);
            decimal provisionParaCuentasIncobrables = _obtenerVariables.Valor("Provisión para Cuentas Incobrables", años, meses);

            decimal resultado =
              (cuentaEnParticipacion
            + cuentasPorCobrarACortoPlazo
            + efectivoEnCaja
            + efectivoEnBancoYEnOtrasInstituciones
            + inversionesACortoPlazoOTemporales
            + efectosPorCobrarACortoPlazo
            + pagosPorCuentasDeTerceros
            + creditosDocumentarios
            + totalInventario) - (efectosPorCobrarDescontados - provisionParaCuentasIncobrables)
            ;
            return resultado;
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
            decimal efectosPorCobrarALargoPlazo = _obtenerVariables.Valor("Efectos por Cobrar a Largo Plazo", años, meses);
            decimal cuentasPorCobrarACortoPlazo = _obtenerVariables.Valor("Cuentas por Cobrar a Largo Plazo", años, meses);
            decimal prestamosConcedidosACobrarALargoPlazo = _obtenerVariables.Valor("Préstamos Concedidos a Cobrar a Largo Plazo", años, meses);
            decimal inversionesALargoPlazoOPermanentes = _obtenerVariables.Valor("Inversiones a Largo Plazo o Permanentes", años, meses);

            decimal resultado =
              efectosPorCobrarALargoPlazo
            + cuentasPorCobrarACortoPlazo
            + prestamosConcedidosACobrarALargoPlazo
            + inversionesALargoPlazoOPermanentes
            ;
            return resultado;
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
            decimal mediosYEquiposParaAlquilar = _obtenerVariables.Valor("Medios y Equipos para Alquilar", años, meses);
            decimal fondosBibliotecarios = _obtenerVariables.Valor("Fondos Bibliotecarios", años, meses);
            decimal aft = _obtenerVariables.Valor("Activos Fijos Tangibles", años, meses);
            decimal monumentosYObrasDeArte = _obtenerVariables.Valor("Monumentos y Obras de Arte", años, meses);
            decimal afi = _obtenerVariables.Valor("Activos Fijos Intangibles", años, meses);
            decimal inversionesEnProceso = _obtenerVariables.Valor("Inversiones en Proceso", años, meses);
            decimal equiposPorInstalarMaterialesParaElProcesoInversionista = _obtenerVariables.Valor("Equipos por Instalar y Materiales para el Proceso Inversionista", años, meses);

            decimal daft = _obtenerVariables.Valor("Depreciación de Activos Fijos Tangibles", años, meses);
            decimal desgasteDeMediosYEquiposParaAlquilar = _obtenerVariables.Valor("Desgaste de Medios y Equipos para Alquilar", años, meses);
            decimal amortizacionDeActivosFijosIntangibles = _obtenerVariables.Valor("Amortización de Activos Fijos Intangibles", años, meses);

            decimal resultado =
              (mediosYEquiposParaAlquilar
            + fondosBibliotecarios
            + aft
            + monumentosYObrasDeArte
            + afi
            + inversionesEnProceso
            + equiposPorInstalarMaterialesParaElProcesoInversionista)
            - (daft - desgasteDeMediosYEquiposParaAlquilar - amortizacionDeActivosFijosIntangibles)
            ;
            return resultado;
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
            decimal gastosDeProduccionServiciosDiferidos = _obtenerVariables.Valor("Gastos de Produccion y Servicios Diferidos", años, meses);
            decimal gastosFinancierosDiferidos = _obtenerVariables.Valor("Gastos Financieros Diferidos", años, meses);
            decimal gastosDiferidosDelProcesoInversionista = _obtenerVariables.Valor("Gastos Diferidos del Proceso Inversionista", años, meses);

            decimal resultado =
              gastosDeProduccionServiciosDiferidos
            + gastosFinancierosDiferidos
            + gastosDiferidosDelProcesoInversionista
            ;
            return resultado;
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
            decimal perdidasEnInvestigacion = _obtenerVariables.Valor("Pérdidas en Investigación", años, meses);

            decimal resultado =
              perdidasEnInvestigacion
            ;
            return resultado;
        }

    }
}