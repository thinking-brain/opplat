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
        ObtenerPlanGI _obtenetPlan { get; set; }

        public ObtenerValuesEnVariablesEstadoFinanciero(FinanzasDbContext context, ObtenerPlanGI obtenerPlan, IConfiguration config)
        {
            _config = config;
            _context = context;
            _obtenetPlan = obtenerPlan;
        }
        public static string Dato(string dato)
        {
            var data = DatosVariablesEstadoFinanciero.Datos().SingleOrDefault(s => s.Dato.Trim().ToUpper().Equals(dato.Trim().ToUpper()));
            return (data.Valor.ToString());
        }
        public decimal Valor(string dato, int year, int meses)
        {
            //todo: hacer la consulta por todo el año y guardarla en memoria para mejora velocidad
            string variable = Dato(dato);
            var variableCuentas = variable.Split(',');
            decimal variableValue = _context.Set<CacheCuentaPeriodo>()
                .Where(c => variableCuentas.Contains(c.Cuenta) && c.Mes == meses && c.Year == year)
                .Sum(c => c.Saldo);
            return variableValue;
        }

        public decimal activoCirculante(int year, int meses)
        {
            decimal cuentaEnParticipacion = Valor("Cuenta en Participación", year, meses);
            decimal cuentasPorCobrarACortoPlazo = Valor("Cuentas por Cobrar a Corto Plazo", year, meses);
            decimal efectivoEnCaja = Valor("Efectivo en Caja", year, meses);
            decimal efectivoEnBancoYEnOtrasInstituciones = Valor("Efectivo en Banco y en otras instituciones", year, meses);
            decimal inversionesACortoPlazoOTemporales = Valor("Inversiones a Corto Plazo o Temporales", year, meses);
            decimal efectosPorCobrarACortoPlazo = Valor("Efectos por Cobrar a Corto Plazo", year, meses);
            decimal pagosPorCuentasDeTerceros = Valor("Pagos por Cuentas de Terceros", year, meses);
            decimal creditosDocumentarios = Valor("Créditos Documentarios", year, meses);
            decimal participacionDeReaseguradorasPorSiniestrosPendientes = Valor("Participación de Reaseguradoras por Siniestros Pendientes", year, meses);
            decimal prestamosYOtrasOperacionesCrediticiasACobrarACortoPlazo = Valor("Préstamos y Otras Operaciones Crediticias a Cobrar a Corto Plazo", year, meses);
            decimal suscriptoresDeBonos = Valor("Suscriptores de Bonos", year, meses);
            decimal pagosAnticipadosASuministradores = Valor("Pagos Anticipados a Suministradores", year, meses);
            decimal pagosAnticipadosDelProcesoInversionista = Valor("Pagos Anticipados del Proceso + Inversionista", year, meses);
            decimal anticiposAJustificar = Valor("Anticipos a Justificar", year, meses);
            decimal adeudosDelPresupuestoDelEstado = Valor("Adeudos del Presupuesto del Estado", year, meses);
            decimal adeudosDelOrganoUOrganismo = Valor("Adeudos del Organo u Organismo", year, meses);
            decimal ingresosAcumuladosPorCobrar = Valor("Ingresos acumulados por Cobrar", year, meses);
            decimal dividendosYParticipacionesOPorCobrar = Valor("Dividendos y Participaciones o por Cobrar", year, meses);
            decimal ingresosAcumuladosPorCobrarReasegurosAceptados = Valor("Ingresos Acumulados por Cobrar - Reaseguros Aceptados", year, meses);
            decimal totalInventario = InventarioTotal(year, meses);

            decimal efectosPorCobrarDescontados = Valor("Efectos por Cobrar Descontados", year, meses);
            decimal provisionParaCuentasIncobrables = Valor("Provisión para Cuentas Incobrables", year, meses);

            decimal resultado =
              (cuentaEnParticipacion
            + cuentasPorCobrarACortoPlazo
            + efectivoEnCaja
            + efectivoEnBancoYEnOtrasInstituciones
            + inversionesACortoPlazoOTemporales
            + efectosPorCobrarACortoPlazo
            + pagosPorCuentasDeTerceros
            + participacionDeReaseguradorasPorSiniestrosPendientes
            + prestamosYOtrasOperacionesCrediticiasACobrarACortoPlazo
            + suscriptoresDeBonos
            + pagosAnticipadosASuministradores
            + pagosAnticipadosDelProcesoInversionista
            + anticiposAJustificar
            + adeudosDelPresupuestoDelEstado
            + adeudosDelOrganoUOrganismo
            + ingresosAcumuladosPorCobrar
            + dividendosYParticipacionesOPorCobrar
            + ingresosAcumuladosPorCobrarReasegurosAceptados
            + creditosDocumentarios
            + totalInventario) - (efectosPorCobrarDescontados + provisionParaCuentasIncobrables)
            ;
            return resultado;
        }
        public decimal activosLargoPlazo(int year, int meses)
        {
            decimal efectosPorCobrarALargoPlazo = Valor("Efectos por Cobrar a Largo Plazo", year, meses);
            decimal cuentasPorCobrarACortoPlazo = Valor("Cuentas por Cobrar a Largo Plazo", year, meses);
            decimal prestamosConcedidosACobrarALargoPlazo = Valor("Préstamos Concedidos a Cobrar a Largo Plazo", year, meses);
            decimal inversionesALargoPlazoOPermanentes = Valor("Inversiones a Largo Plazo o Permanentes", year, meses);

            decimal resultado =
              efectosPorCobrarALargoPlazo
            + cuentasPorCobrarACortoPlazo
            + prestamosConcedidosACobrarALargoPlazo
            + inversionesALargoPlazoOPermanentes
            ;
            return resultado;
        }
        public decimal activosFijos(int year, int meses)
        {
            decimal mediosYEquiposParaAlquilar = Valor("Medios y Equipos para Alquilar", year, meses);
            decimal fondosBibliotecarios = Valor("Fondos Bibliotecarios", year, meses);
            decimal aft = Valor("Activos Fijos Tangibles", year, meses);
            decimal monumentosYObrasDeArte = Valor("Monumentos y Obras de Arte", year, meses);
            decimal afi = Valor("Activos Fijos Intangibles", year, meses);
            decimal inversionesEnProceso = Valor("Inversiones en Proceso", year, meses);
            decimal equiposPorInstalarMaterialesParaElProcesoInversionista = Valor("Equipos por Instalar y Materiales para el Proceso Inversionista", year, meses);

            decimal daft = Valor("Depreciación de Activos Fijos Tangibles", year, meses);
            decimal desgasteDeMediosYEquiposParaAlquilar = Valor("Desgaste de Medios y Equipos para Alquilar", year, meses);
            decimal amortizacionDeActivosFijosIntangibles = Valor("Amortización de Activos Fijos Intangibles", year, meses);

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
        public decimal activosDiferidos(int year, int meses)
        {
            decimal gastosDeProduccionServiciosDiferidos = Valor("Gastos de Produccion y Servicios Diferidos", year, meses);
            decimal gastosFinancierosDiferidos = Valor("Gastos Financieros Diferidos", year, meses);
            decimal gastosDiferidosDelProcesoInversionista = Valor("Gastos Diferidos del Proceso Inversionista", year, meses);

            decimal resultado =
              gastosDeProduccionServiciosDiferidos
            + gastosFinancierosDiferidos
            + gastosDiferidosDelProcesoInversionista
            ;
            return resultado;
        }
        public decimal otrosActivos(int year, int meses)
        {
            decimal perdidasEnInvestigacion = Valor("Pérdidas en Investigación", year, meses);
            decimal faltanteDeBienesEnInvestigacion = Valor("Faltante de Bienes en Investigación", year, meses);
            decimal cuentasPorCobrarDiversasOperacionesCorrientes = Valor("Cuentas por Cobrar Diversas - Operaciones Corrientes", year, meses);
            decimal cuentasPorCobrarCompraDeMonedas = Valor("Cuentas por Cobrar - Compra de Monedas", year, meses);
            decimal cuentasPorCobrarDiversaDelProcesoInversionista = Valor("Cuentas por Cobrar Diversa del Proceso Inversionista", year, meses);
            decimal efectosPorCobrarEnLitigio = Valor("Efectos por Cobrar en Litigio", year, meses);
            decimal cuentasPorCobrarEnLitigio = Valor("Cuentas por Cobrar en Litigio", year, meses);
            decimal efectosPorCobrarProtestados = Valor("Efectos por Cobrar Protestados", year, meses);
            decimal cuentasPorCobrarEnProcesoJudicial = Valor("Cuentas por Cobrar en Proceso Judicial", year, meses);
            decimal depositosYFianzas = Valor("Depósitos y Fianzas", year, meses);
            decimal fondoDeAmortizacionDeBonosEfectivoEnValores = Valor("Fondo de Amortización de Bonos - Efectivo en Valores", year, meses);
            decimal menosOtrasProvisionesReguladorasDeActivos = Valor("Otras Provisiones Reguladoras de Activos", year, meses);
            decimal resultado =
              perdidasEnInvestigacion
              + faltanteDeBienesEnInvestigacion
              + cuentasPorCobrarDiversasOperacionesCorrientes
              + cuentasPorCobrarCompraDeMonedas
              + cuentasPorCobrarDiversaDelProcesoInversionista
              + efectosPorCobrarEnLitigio
              + cuentasPorCobrarEnLitigio
              + efectosPorCobrarProtestados
              + cuentasPorCobrarEnProcesoJudicial
              + depositosYFianzas
              + fondoDeAmortizacionDeBonosEfectivoEnValores
              - menosOtrasProvisionesReguladorasDeActivos
            ;
            return resultado;
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
            decimal menosDesgasteDeBaseMaterialDeEstudio = Valor("Desgaste de Base Material de Estudio", year, meses);
            decimal menosDesgasteDeVestuarioYLenceria = Valor("Desgaste de Vestuario y Lenceria", year, meses);


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
            - (desgasteDeUtilesYHerramientas + menosDesgasteDeBaseMaterialDeEstudio + menosDesgasteDeVestuarioYLenceria)
            ;
            return resultado;
        }
        public decimal pasivoCirculante(int year, int meses)
        {
            decimal sobregiroBancario = Valor("Sobregiro Bancario", year, meses);
            decimal efectosPorPagarACortoPlazo = Valor("Efectos por Pagar a Corto Plazo", year, meses);
            decimal cuentasPorPagarACortoPlazo = Valor("Cuentas por Pagar a Corto Plazo", year, meses);
            decimal cobrosPorCuentaDeTerceros = Valor("Cobros por Cuenta de Terceros", year, meses);
            decimal dividendosYParticipacionesPorPagar = Valor("Dividendos y Participaciones por Pagar", year, meses);
            decimal cuentasEnParticipacion = Valor("Cuentas en Participación", year, meses);
            decimal cuentasPorPagarActivosFijosTangibles = Valor("Cuentas por Pagar-Activos Fijos Tangibles", year, meses);
            decimal cuentasPorPagarDelProcesoInversionista = Valor("Cuentas por Pagar del Proceso Inversionista", year, meses);
            decimal cobrosAnticipados = Valor("Cobros Anticipados", year, meses);
            decimal depositosRecibidos = Valor("Depósitos Recibidos", year, meses);
            decimal obligacionesConElPresupuestoDelEstado = Valor("Obligaciones con el Presupuesto del Estado", year, meses);
            decimal obligacionesConElOrganoUOrganismo = Valor("Obligaciones con el Organo u Organismo", year, meses);
            decimal nominasPorPagar = Valor("Nóminas por Pagar", year, meses);
            decimal retencionesPorPagar = Valor("Retenciones por Pagar", year, meses);
            decimal prestamosRecibidosOtrasOperacionesCrediticiasPorPagar = Valor("Préstamos Recibidos y Otras Operaciones Crediticias por Pagar", year, meses);
            decimal gastosAcumuladosPorPagar = Valor("Gastos Acumulados por Pagar", year, meses);
            decimal provisionParaVacaciones = Valor("Provisión para Vacaciones", year, meses);
            decimal otrasProvosionesOperacionales = Valor("Otras Provosiones Operacionales", year, meses);
            decimal provisionParaPagosDeLosSubsidiosDeSeguridadSocialACortoPlazo = Valor("Provisión para Pagos de los Subsidios de Seguridad Social a Corto Plazo", year, meses);
            decimal fondoDeCompensacionParaDesbalancesFinancieros = Valor("Fondo de Compensación para Desbalances Financieros", year, meses);

            decimal resultado =
              sobregiroBancario
            + efectosPorPagarACortoPlazo
            + cuentasPorPagarACortoPlazo
            + cobrosPorCuentaDeTerceros
            + dividendosYParticipacionesPorPagar
            + cuentasEnParticipacion
            + cuentasPorPagarActivosFijosTangibles
            + cuentasPorPagarDelProcesoInversionista
            + cobrosAnticipados
            + depositosRecibidos
            + obligacionesConElPresupuestoDelEstado
            + obligacionesConElOrganoUOrganismo
            + nominasPorPagar
            + retencionesPorPagar
            + prestamosRecibidosOtrasOperacionesCrediticiasPorPagar
            + gastosAcumuladosPorPagar
            + provisionParaVacaciones
            + otrasProvosionesOperacionales
            + provisionParaPagosDeLosSubsidiosDeSeguridadSocialACortoPlazo
            + fondoDeCompensacionParaDesbalancesFinancieros
            ;
            return resultado;
        }
        public decimal pasivosALargoPlazo(int year, int meses)
        {

            decimal efectosPorPagarALargoPlazo = Valor("Efectos por Pagar a Largo Plazo", year, meses);
            decimal cuentasPorPagarALargoPlazo = Valor("Cuentas por Pagar a Largo Plazo", year, meses);
            decimal prestamosRecibidosPorPagarALargoPlazo = Valor("Préstamos Recibidos por Pagar a Largo Plazo", year, meses);
            decimal obligacionesALargoPlazo = Valor("Obligaciones a Largo Plazo", year, meses);
            decimal otrasProvisionesALargoPlazo = Valor("Otras Provisiones a Largo Plazo", year, meses);
            decimal bonosPorPagar = Valor("Bonos por Pagar", year, meses);
            decimal bonosSuscritos = Valor("Bonos Suscritos", year, meses);

            decimal resultado =
              efectosPorPagarALargoPlazo
            + cuentasPorPagarALargoPlazo
            + prestamosRecibidosPorPagarALargoPlazo
            + obligacionesALargoPlazo
            + otrasProvisionesALargoPlazo
            + bonosPorPagar
            + bonosSuscritos
            ;
            return resultado;
        }
        public decimal pasivosDiferidos(int year, int meses)
        {
            decimal ingresosDiferidos = Valor("Ingresos Diferidos", year, meses);
            decimal ingresosDiferidosPorDonacionesRecibidas = Valor("Ingresos Diferidos por Donaciones Recibidas", year, meses);

            decimal resultado =
              ingresosDiferidos
            + ingresosDiferidosPorDonacionesRecibidas
            ;
            return resultado;
        }
        public decimal otrosPasivos(int year, int meses)
        {
            decimal sobrantesEnInvestigacion = Valor("Sobrantes en Investigación", year, meses);
            decimal cuentasPorPagarDiversas = Valor("Cuentas por Pagar Diversas", year, meses);
            decimal cuentasPorPagarCompraDeMoneda = Valor("Cuentas por Pagar - Compra de Moneda", year, meses);
            decimal ingresosDePeriodosFuturos = Valor("Ingresos de Períodos Futuros", year, meses);

            decimal resultado =
              sobrantesEnInvestigacion
            + cuentasPorPagarDiversas
            + cuentasPorPagarCompraDeMoneda
            + ingresosDePeriodosFuturos
            ;
            return resultado;
        }
        public decimal efectivoEnCaja(int year, int meses)
        {
            decimal valor = Valor("Efectivo en Caja", year, meses);

            decimal resultado =
              valor
            ;
            return resultado;
        }
        public decimal efectivoEnBanco(int year, int meses)
        {
            decimal valor = Valor("Efectivo en Banco y en otras instituciones", year, meses);

            decimal resultado =
              valor
            ;
            return resultado;
        }
        public decimal totalDePatrimonioNeto(int year, int meses)
        {
            decimal inversionEstatal = Valor("Inversión Estatal", year, meses);
            decimal sectorPublicoPatrimonio = Valor("Sector Público + Patrimonio", year, meses);
            decimal sectorPrivadoCapitalSocialSuscritoPagadoRecursosRecibidos = Valor("Sector Privado + Capital Social Suscrito y Pagado + Recursos Recibidos", year, meses);
            decimal sectorPublicoDonacionesRecibidasNacionales = Valor("Sector Publico + Donaciones Recibidas - Nacionales", year, meses);
            decimal donacionesRecibidasExterior = Valor("Donaciones Recibidas - Exterior", year, meses);
            decimal utilidadRetenida = Valor("Utilidad Retenida", year, meses);
            decimal subvencionPorPerdidaSectorPublico = Valor("Subvención por Pérdida Sector Publico", year, meses);
            decimal ReservasParaContingencias = Valor("Reservas para Contingencias", year, meses);
            decimal otrasReservasPatrimoniales = Valor("Otras Reservas Patrimoniales", year, meses);

            decimal recursosEntregados = Valor("Recursos Entregados", year, meses);
            decimal sectorPublicoDonacionesEntregadasNacionales = Valor("Sector Publico + Donaciones Entregadas - Nacionales", year, meses);
            decimal donacionesEntregadasExterior = Valor("Donaciones Entregadas - Exterior", year, meses);

            decimal pagoACuentaDeDividendos = Valor("Pago a Cuenta de Dividendos", year, meses);
            decimal pagoACuentaDeUtilidades = Valor("Pago a Cuenta de Utilidades", year, meses);
            decimal perdida = Valor("Pérdida", year, meses);
            decimal masoMenosReevalucionDeActivosFijosTangibles = Valor("Reevalucion de Activos Fijos Tangibles", year, meses);
            decimal otrasOperacionesDeCapital = Valor("Otras Operaciones de Capital", year, meses);
            decimal sectorPrivadoRevaluaciónDeInventarios = Valor("Sector Privado Revaluación de Inventarios", year, meses);
            decimal gananciaOPerdidaNoRealizada = Valor("Ganancia o Pérdida no Realizada", year, meses);

            decimal resultado =
              (
                  inversionEstatal
              + sectorPublicoPatrimonio
              + sectorPrivadoCapitalSocialSuscritoPagadoRecursosRecibidos
              + sectorPublicoDonacionesRecibidasNacionales
              + donacionesRecibidasExterior
              + utilidadRetenida
              + subvencionPorPerdidaSectorPublico
              + ReservasParaContingencias
              + otrasReservasPatrimoniales
              )
            - (
                recursosEntregados
                + sectorPublicoDonacionesEntregadasNacionales
                + donacionesEntregadasExterior
            )
            +
            (
                pagoACuentaDeUtilidades
                + pagoACuentaDeDividendos
                + perdida
                + masoMenosReevalucionDeActivosFijosTangibles
                + otrasReservasPatrimoniales
                + sectorPrivadoRevaluaciónDeInventarios
                + gananciaOPerdidaNoRealizada
            )
            ;
            return resultado;
        }

        /// <summary>
        /// Devuelve los ingresos de las Ventas en un mes y un año
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        public decimal ObtenerVentas(int year, int mes)
        {
            var plan = new List<PlanGIViewModel>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanIngresosPeriodo.Get(year, _config);
            var ventasMN = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == "912" && c.Mes == mes && c.Year == year);
            var ventasCUC = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == "913" && c.Mes == mes && c.Year == year);
            decimal importeMN = 0;
            if (ventasMN != null)
            {
                importeMN = ventasMN.Saldo;
            }
            decimal importeCUC = 0;
            if (ventasCUC != null)
            {
                importeCUC = ventasCUC.Saldo;
            }

            decimal resultado = importeMN + importeCUC;
            return resultado;
        }


        public decimal ObtenerSolvenciaFinanciera(int años, int meses)
        {
            decimal ingresos = _obtenetPlan.ObtenerTotalIngresos(años, meses);
            decimal egresos = _obtenetPlan.ObtenerTotalEgresos(años, meses);
            decimal solvenciaFinanciera = egresos != 0 ? ingresos / egresos : 0;
            return Math.Round(solvenciaFinanciera, 2);
        }

        //Capital de Trabajo
        public decimal ObtenerCapitalDeTrabajo(int años, int meses)
        {
            decimal ac = activoCirculante(años, meses);
            decimal pc = pasivoCirculante(años, meses);
            decimal capitalDeTrabajo = ac - pc;
            return Math.Round(capitalDeTrabajo, 2);
        }

        //Indice de Disponibilidad
        public decimal ObtenerIndiceDeDisponibilidad(int años, int meses)
        {
            decimal ac = activoCirculante(años, meses);
            decimal pc = pasivoCirculante(años, meses);
            decimal ec = efectivoEnCaja(años, meses);
            decimal eb = efectivoEnBanco(años, meses);
            decimal indiceDeDisponibilidad = ac != 0 ? (ec + eb) / ac : 0;
            return Math.Round(indiceDeDisponibilidad, 2);
        }

        //Indice de Liquidez General
        public decimal ObtenerIndiceDeLiquidez(int años, int meses)
        {
            decimal ac = activoCirculante(años, meses);
            decimal pc = pasivoCirculante(años, meses);
            decimal indiceDeLiquidezGneral = pc != 0 ? ac / pc : 0;
            return Math.Round(indiceDeLiquidezGneral);
        }

        //Liquidez de Tesorería
        public decimal ObtenerLiquidezDeTesoreria(int años, int meses)
        {
            decimal ec = efectivoEnCaja(años, meses);
            decimal eb = efectivoEnBanco(años, meses);
            decimal ac = activoCirculante(años, meses);
            decimal pc = pasivoCirculante(años, meses);
            decimal al = ec + eb;
            decimal liquidezDeTesoreria = pc != 0 ? al / pc : 0;
            return Math.Round(liquidezDeTesoreria, 2);
        }

        //Indice de Deuda o Razon de Endeudamiento
        public decimal ObtenerIndiceDeDeudaRazonDeEndeudamiento(int años, int meses)
        {
            decimal ac = activoCirculante(años, meses);
            decimal pc = pasivoCirculante(años, meses);
            decimal plp = pasivosALargoPlazo(años, meses);
            decimal pd = pasivosDiferidos(años, meses);
            decimal op = otrosPasivos(años, meses);
            decimal tp = pc + plp + pd + op;

            decimal ad = activosDiferidos(años, meses);
            decimal af = activosFijos(años, meses);
            decimal alp = activosLargoPlazo(años, meses);
            decimal oa = otrosActivos(años, meses);
            decimal ta = ac + ad + af + alp + oa;

            decimal indiceDeDeudaORazonDeEndeudamiento = ta != 0 ? tp / ta : 0;
            return Math.Round(indiceDeDeudaORazonDeEndeudamiento, 2);
        }

        //Margen de utilidad 
        public decimal ObtenerMargenDeUtilidad(int años, int meses)
        {
            decimal ingresos = _obtenetPlan.ObtenerTotalIngresos(años, meses);
            decimal egresos = _obtenetPlan.ObtenerTotalEgresos(años, meses);
            decimal utilidadAntesDeImpuesto = ingresos - egresos;
            decimal ventas = ObtenerVentas(años, meses);
            decimal margenDeUtilidad = ventas != 0 ? utilidadAntesDeImpuesto / ventas : 0;
            return Math.Round(margenDeUtilidad, 2);
        }
        //Rentabilidad Económica"
        public decimal ObtenerRentabilidadEconomica(int años, int meses)
        {
            decimal ad = activosDiferidos(años, meses);
            decimal af = activosFijos(años, meses);
            decimal alp = activosLargoPlazo(años, meses);
            decimal oa = otrosActivos(años, meses);
            decimal ac = activoCirculante(años, meses);
            decimal ingresos = _obtenetPlan.ObtenerTotalIngresos(años, meses);
            decimal egresos = _obtenetPlan.ObtenerTotalEgresos(años, meses);
            decimal utilidadAntesDeImpuesto = ingresos - egresos;
            decimal totalDeActivosPromedio = ((af + ac + ad + alp + oa) / 1);
            decimal rentabilidadEconomica = totalDeActivosPromedio != 0 ? utilidadAntesDeImpuesto / totalDeActivosPromedio : 0;
            return Math.Round(rentabilidadEconomica, 2);
        }

        //Rentabilidad Financiera
        public decimal ObtenerRentabilidadFinanciera(int años, int meses)
        {
            decimal patrimonioNeto = totalDePatrimonioNeto(años, meses);
            decimal ingresos = _obtenetPlan.ObtenerTotalIngresos(años, meses);
            decimal egresos = _obtenetPlan.ObtenerTotalEgresos(años, meses);
            decimal utilidadAntesDeImpuesto = ingresos - egresos;
            decimal rentabilidadFinanciera = patrimonioNeto != 0 ? utilidadAntesDeImpuesto / patrimonioNeto : 0;
            return Math.Round(rentabilidadFinanciera, 2);
        }
    }
}
