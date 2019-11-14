using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using FinanzasWebApi.Models;

namespace FinanzasWebApi.Helper
{
    public class ObtenerValuesEnVariablesEstadoFinanciero
    {
        IConfiguration _config { get; set; }
        FinanzasDbContext _context;

        public ObtenerValuesEnVariablesEstadoFinanciero(FinanzasDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        public static string Dato(string dato)
        {
            var data = DatosVariablesEstadoFinanciero.Datos().SingleOrDefault(s => s.Dato.Trim().ToUpper().Equals(dato.Trim().ToUpper()));
            return (data.Valor.ToString());
        }
        public decimal Valor(string dato, int year, int meses)
        {
            string variable = Dato(dato);
            string[] variableCuentas = variable.Split(',');
            decimal variableValue = 0;
            for (int i = 0; i < variableCuentas.Length; i++)
            {
                string cta = variableCuentas[i].ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                // decimal Importe = 0;
                variableValue = variableValue + Importe;
            }
            return variableValue;
        }

        public decimal InventarioTotal(int year, int meses)
        {
            decimal materiasPrimasYMateriales = Valor("Materias Primas y Materiales", year, meses);
            decimal combustiblesYLubricantes = Valor("Combustibles y Lubricantes", year, meses);
            decimal partesYPiezasDeRespuesto = Valor("Partes y Piezas de Respuesto", year, meses);
            decimal envasesYEmbalajes = Valor("Envases y Embalajes", year, meses);
            decimal utilesHerramientasYOtros = Valor("Utiles, Herramientas y Otros", year, meses);
            decimal produccionTerminada = Valor("Producción Terminada", year, meses);
            decimal mercanciasParaLaVenta = Valor("Mercancias para la Venta", year, meses);
            decimal medicamentos = Valor("Medicamentos", year, meses);
            decimal baseMaterialDeEstudio = Valor("Base Material de Estudio", year, meses);
            decimal vestuarioYLenceria = Valor("Vestuario y Lencería", year, meses);
            decimal alimentos = Valor("Alimentos", year, meses);
            decimal inventarioDeMercanciasDeImportacion = Valor("Inventario de Mercancías de Importación", year, meses);
            decimal inventarioDeMercanciasDeExportación = Valor("Inventario de Mercancias de Exportación", year, meses);
            decimal produccionesParaInsumoOAutoconsumo = Valor("Producciones para Insumo o Autoconsumo", year, meses);
            decimal otrosInventarios = Valor("Otros Inventarios", year, meses);
            decimal inventariosOciosos = Valor("Inventarios Ociosos", year, meses);
            decimal inventariosDeLentoMovimiento = Valor("Inventarios de lento Movimiento", year, meses);
            decimal produccionEnProceso = Valor("Producción en Proceso", year, meses);
            decimal produccionPropiaParaInsumoOAutoconsumo = Valor("Produccion Propia para Insumo o Autoconsumo", year, meses);
            decimal reparacionesCapitalesConMediosPropios = Valor("Reparaciones Capitales con Medios Propios", year, meses);
            decimal inversionesConMediosPropios = Valor("Inversiones con Medios Propios", year, meses);


            decimal desgasteDeUtilesYHerramientas = Valor("Desgaste de Utiles y Herramientas", year, meses);
            decimal menosDesgasteDeBaseMaterialDeEstudio = Valor("Menos: Desgaste de Base Material de Estudio", year, meses);
            decimal menosDesgasteDeVestuarioYLenceria = Valor("Menos: Desgaste de Vestuario y Lenceria", year, meses);


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

    }
}
