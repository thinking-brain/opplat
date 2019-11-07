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
    public class ObtenerPlanGI
    {
        IConfiguration _config { get; set; }
        FinanzasDbContext _context;

        public ObtenerPlanGI(IConfiguration config, FinanzasDbContext context)
        {
            _config = config;
            _context = context;
        }

        public List<PlanGIViewModel> ObtenerIngresos(int year, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanIngresosPeriodo.Get(year, _config);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Ingresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                decimal ImporteAcumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
                var planEnMes = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto))[meses] : 0M;
                var planAcum = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto)).Acumulado(meses) : 0M;

                var PlanMes = planEnMes;
                var PlanAcumulado = planAcum;
                var RealMes = Importe;
                var RealAcumulado = ImporteAcumulado;

                var cost = new PlanGIViewModel
                {
                    //Grupo
                    Grupo = item.Dato.ToString(),
                    //Plan Mensual
                    PlanMes = PlanMes,
                    //Real del Mes
                    RealMes = RealMes,

                    // % de Cumplimiento
                    PorcCumplimiento = PlanMes > 0 ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos
                    PorcRelacionIngresos = null,
                    //% de los gastos en función del total
                    PorcGastosFuncionTotal = null,
                    //Plan Acumulado
                    PlanAcumulado = PlanAcumulado,
                    //Real Acumulado
                    RealAcumulado = RealAcumulado,
                    //% Cumplimiento
                    PorcCumpAcumulado = PlanAcumulado > 0 ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos 
                    PorcIngresosFuncionTotal = null,
                    //% de los gastos en función del total 
                    PorcGastosFuncionTotalAcumulado = null,
                    ////Total en el Grupo
                    //

                };
                plan.Add(cost);
            }
            plan.Insert(0, new PlanGIViewModel
            {
                //Grupo
                Grupo = "Ingresos",
                //Plan Mensual
                PlanMes = plan.Sum(p => p.PlanMes),
                //Real del Mes
                RealMes = plan.Sum(p => p.RealMes),

                // % de Cumplimiento
                PorcCumplimiento = plan.Sum(p => p.PlanMes) > 0 ? Math.Round((plan.Sum(p => p.RealMes) / plan.Sum(p => p.PlanMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                // % en relación a ingresos
                PorcRelacionIngresos = null,
                //% de los gastos en función del total
                PorcGastosFuncionTotal = null,
                //Plan Acumulado
                PlanAcumulado = plan.Sum(p => p.PlanAcumulado),
                //Real Acumulado
                RealAcumulado = plan.Sum(p => p.RealAcumulado),
                //% Cumplimiento
                PorcCumpAcumulado = plan.Sum(p => p.PlanAcumulado) > 0 ? Math.Round((plan.Sum(p => p.RealAcumulado) / plan.Sum(p => p.PlanAcumulado)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                // % en relación a ingresos 
                PorcIngresosFuncionTotal = null,
                //% de los gastos en función del total 
                PorcGastosFuncionTotalAcumulado = null,
                ////Total en el Grupo
                //

            });
            return plan;

        }
        public decimal ObtenerTotalIngresos(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Ingresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = Importe,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }
        public decimal ObtenerTotalPlanIngresos(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanIngresosPeriodo.Get(year, _config);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Ingresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                var planEnMes = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto))[meses] : 0M;
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = planEnMes,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }
        public decimal ObtenerTotalIngresosAcumulados(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Ingresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = Importe,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }

        public List<PlanGIViewModel> ObtenerEgresos(int year, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanEgresosPeriodo.Get(year, _config);
            var TotalIngresosEnMes = ObtenerTotalIngresos(year, meses);
            var TotalEgresosEnMes = ObtenerTotalEgresos(year, meses);
            var TotalIngresosAcumulados = ObtenerTotalIngresosAcumulados(year, meses);
            var TotalEgresosAcumuados = ObtenerTotalEgresosAcumulados(year, meses);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Egresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                decimal ImporteAcumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
                var planEnMes = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto))[meses] : 0M;
                var planAcum = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto)).Acumulado(meses) : 0M;

                var PlanMes = planEnMes;
                var PlanAcumulado = planAcum;
                var RealMes = Importe;
                var RealAcumulado = ImporteAcumulado;

                var cost = new PlanGIViewModel
                {
                    //Grupo
                    Grupo = item.Dato.ToString(),
                    //Plan Mensual
                    PlanMes = PlanMes,
                    //Real del Mes
                    RealMes = RealMes,

                    // % de Cumplimiento
                    PorcCumplimiento = PlanMes > 0 ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos
                    PorcRelacionIngresos = (TotalIngresosEnMes > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    //% de los gastos en función del total
                    PorcGastosFuncionTotal = TotalEgresosEnMes > 0 ? Math.Round((RealMes / TotalEgresosEnMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    //Plan Acumulado
                    PlanAcumulado = PlanAcumulado,
                    //Real Acumulado
                    RealAcumulado = RealAcumulado,
                    //
                    PorcCumpAcumulado = PlanAcumulado > 0 ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                    // % en relación a ingresos 
                    PorcIngresosFuncionTotal = TotalIngresosAcumulados > 0 ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                    //% de los gastos en función del total 
                    PorcGastosFuncionTotalAcumulado = TotalEgresosAcumuados > 0 ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                };
                plan.Add(cost);
            }
            plan.Insert(0, new PlanGIViewModel
            {
                //Grupo
                Grupo = "Egresos",
                //Plan Mensual
                PlanMes = plan.Sum(p => p.PlanMes),
                //Real del Mes
                RealMes = plan.Sum(p => p.RealMes),

                // % de Cumplimiento
                PorcCumplimiento = plan.Sum(p => p.PlanMes) > 0 ? Math.Round((plan.Sum(p => p.RealMes) / plan.Sum(p => p.PlanMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                // % en relación a ingresos
                PorcRelacionIngresos = (TotalIngresosEnMes > 0 && plan.Sum(p => p.RealMes) > 0) ? Math.Round((plan.Sum(p => p.RealMes) / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                //% de los gastos en función del total
                PorcGastosFuncionTotal = TotalEgresosEnMes > 0 ? Math.Round((plan.Sum(p => p.RealMes) / TotalEgresosEnMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                //Plan Acumulado
                PlanAcumulado = plan.Sum(p => p.PlanAcumulado),
                //Real Acumulado
                RealAcumulado = plan.Sum(p => p.RealAcumulado),
                //
                PorcCumpAcumulado = plan.Sum(p => p.PlanAcumulado) > 0 ? Math.Round((plan.Sum(p => p.RealAcumulado) / plan.Sum(p => p.PlanAcumulado)) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                // % en relación a ingresos 
                PorcIngresosFuncionTotal = TotalIngresosAcumulados > 0 ? Math.Round((plan.Sum(p => p.RealMes) / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                //% de los gastos en función del total 
                PorcGastosFuncionTotalAcumulado = TotalEgresosAcumuados > 0 ? Math.Round((plan.Sum(p => p.RealMes) / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,

            });
            return plan;

        }
        public decimal ObtenerTotalEgresos(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Egresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = Importe,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }
        public decimal ObtenerTotalPlanEgresos(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanIngresosPeriodo.Get(year, _config);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Egresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                var planEnMes = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto))[meses] : 0M;
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = planEnMes,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }
        public decimal ObtenerTotalEgresosAcumulados(int year, int meses)
        {
            var plan = new List<MovimientoCuentaPeriodoVM>();
            var modelo = DatosPlanGI.Datos();
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Egresos")))
            {
                string cta = item.Valor.ToString();
                string concepto = item.Dato.ToString();
                decimal Importe = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
                var cost = new MovimientoCuentaPeriodoVM
                {
                    Cuenta = cta,
                    Importe = Importe,
                };
                plan.Add(cost);
            }

            return plan.Sum(s => s.Importe);

        }

        public List<PlanGIViewModel> ObtenerUtilidad(int year, int meses)
        {
            var plan = new List<PlanGIViewModel>();
            var modelo = DatosPlanGI.Datos();
            var planes = GetPlanUtilidadPeriodo.Get(year, _config);
            var TotalIngresosEnMes = ObtenerTotalIngresos(year, meses);
            var TotalEgresosEnMes = ObtenerTotalEgresos(year, meses);
            var TotalIngresosAcumulados = ObtenerTotalIngresosAcumulados(year, meses);
            var TotalEgresosAcumuados = ObtenerTotalEgresosAcumulados(year, meses);

            //Utilidad
            var utilidad = TotalIngresosEnMes - TotalEgresosEnMes;
            var utilidadAcumulada = TotalIngresosAcumulados - TotalEgresosAcumuados;
            var planUtilidad = planes.Any(p => p.Concepto == "Utilidad") ? planes.SingleOrDefault(p => p.Concepto == "Utilidad")[meses] : 0;
            var planUtilidadAcumulado = planes.Any(p => p.Concepto == "Utilidad") ? planes.SingleOrDefault(p => p.Concepto == "Utilidad").Acumulado(meses) : 0;
            plan.Add(new PlanGIViewModel
            {
                Grupo = "Utilidad",
                PlanMes = Math.Round(planUtilidad, 2),
                RealMes = Math.Round(utilidad, 2),
                PorcCumplimiento = planUtilidad != 0 ? Math.Round(utilidad * 100 / planUtilidad, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((utilidad / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planUtilidadAcumulado, 2),
                RealAcumulado = Math.Round(utilidadAcumulada, 2),
                PorcCumpAcumulado = planUtilidadAcumulado != 0 ? Math.Round(utilidadAcumulada * 100 / planUtilidadAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((utilidadAcumulada / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });
            //pago a cargo de utilidad
            var planPago = planes.Any(p => p.Concepto == "Pago a cargo de la utilidad") ? planes.SingleOrDefault(p => p.Concepto == "Pago a cargo de la utilidad")[meses] : 0;
            var planPagoAcumulado = planes.Any(p => p.Concepto == "Pago a cargo de la utilidad") ? planes.SingleOrDefault(p => p.Concepto == "Pago a cargo de la utilidad").Acumulado(meses) : 0;
            var cta = "693";
            decimal pago = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
            decimal pagoAcumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
            plan.Add(new PlanGIViewModel
            {
                Grupo = "Pago a cargo de la utilidad",
                PlanMes = Math.Round(planPago, 2),
                RealMes = Math.Round(pago, 2),
                PorcCumplimiento = planPago != 0 ? Math.Round(pago * 100 / planPago, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((pago / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planPagoAcumulado, 2),
                RealAcumulado = Math.Round(pagoAcumulado, 2),
                PorcCumpAcumulado = planPagoAcumulado != 0 ? Math.Round(pagoAcumulado * 100 / planPagoAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((pagoAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });

            //utilidad despues de pago
            var planUtilidadPago = planUtilidad - planPago;
            var planUtilidadPagoAcumulado = planUtilidadAcumulado - planPagoAcumulado;
            var utilidadPago = utilidad - pago;
            var utilidadPagoAcumulado = utilidadAcumulada - pagoAcumulado;

            plan.Add(new PlanGIViewModel
            {
                Grupo = "Utilidad despues de pagos de anticipos",
                PlanMes = Math.Round(planUtilidadPago, 2),
                RealMes = Math.Round(utilidadPago, 2),
                PorcCumplimiento = planUtilidadPago != 0 ? Math.Round(utilidadPago * 100 / planUtilidadPago, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((utilidadPago / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planUtilidadPagoAcumulado, 2),
                RealAcumulado = Math.Round(utilidadPagoAcumulado, 2),
                PorcCumpAcumulado = planUtilidadPagoAcumulado != 0 ? Math.Round(utilidadPagoAcumulado * 100 / planUtilidadPagoAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((utilidadPagoAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });
            //Reserva de contingencia del 2% al 10%
            var porcientoConfig = _context.Set<ConfiguracionPorciento>().SingleOrDefault(c => c.Titulo == "PorcientoContingencia");
            var porcientoContingencia = porcientoConfig == null ? 2m : (decimal)porcientoConfig.Porciento;
            var porcientoContingenciaPorMes = Math.Round(porcientoContingencia / 12m, 3);
            var planContingencia = (planes.Any(p => p.Concepto == "Egresos") ? planes.SingleOrDefault(p => p.Concepto == "Egresos")[meses] : 0) * porcientoContingenciaPorMes / 100;
            var planContingenciaAcumulado = (planes.Any(p => p.Concepto == "Egresos") ? planes.SingleOrDefault(p => p.Concepto == "Egresos").Acumulado(meses) : 0) * (porcientoContingenciaPorMes * meses) / 100;
            var contingencia = TotalEgresosEnMes * porcientoContingenciaPorMes / 100;
            var contingenciaAcumulada = TotalEgresosAcumuados * (porcientoContingenciaPorMes * meses) / 100;

            plan.Add(new PlanGIViewModel
            {
                Grupo = "Reserva del 2% al 10%",
                PlanMes = Math.Round(planContingencia, 2),
                RealMes = Math.Round(contingencia, 2),
                PorcCumplimiento = planContingencia != 0 ? Math.Round(contingencia * 100 / planContingencia, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((contingencia / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planContingenciaAcumulado, 2),
                RealAcumulado = Math.Round(contingenciaAcumulada, 2),
                PorcCumpAcumulado = planUtilidadPagoAcumulado != 0 ? Math.Round(utilidadPagoAcumulado * 100 / planUtilidadPagoAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((utilidadPagoAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });
            //utilidad despues de reserva de contingencia
            var planDespuesContingencia = planUtilidadPago - planContingencia;
            var planDespuesContingenciaAcumulado = planUtilidadPagoAcumulado - planContingenciaAcumulado;
            var despuesContingencia = utilidadPago - contingencia;
            var despuesContingenciaAcumulado = utilidadPagoAcumulado - contingenciaAcumulada;
            plan.Add(new PlanGIViewModel
            {
                Grupo = "Utilidad libre despues de la reserva",
                PlanMes = Math.Round(planDespuesContingencia, 2),
                RealMes = Math.Round(despuesContingencia, 2),
                PorcCumplimiento = planDespuesContingencia != 0 ? Math.Round(despuesContingencia * 100 / planDespuesContingencia, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((despuesContingencia / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planDespuesContingenciaAcumulado, 2),
                RealAcumulado = Math.Round(despuesContingenciaAcumulado, 2),
                PorcCumpAcumulado = planDespuesContingenciaAcumulado != 0 ? Math.Round(despuesContingenciaAcumulado * 100 / planDespuesContingenciaAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((despuesContingenciaAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });
            //reserva 30%
            var porcientoReserva = 30m;
            var porcientoReservaPorMes = Math.Round(porcientoReserva / 12m, 3);
            var planReserva = planDespuesContingencia * porcientoReservaPorMes / 100;
            var planReservaAcumulado = planDespuesContingenciaAcumulado * (porcientoReservaPorMes * meses) / 100;
            var reserva = TotalEgresosEnMes * porcientoReservaPorMes / 100;
            var reservaAcumulado = TotalEgresosAcumuados * (porcientoReservaPorMes * meses) / 100;
            plan.Add(new PlanGIViewModel
            {
                Grupo = "Reserva de contingencia 30%",
                PlanMes = Math.Round(planReserva, 2),
                RealMes = Math.Round(reserva, 2),
                PorcCumplimiento = planReserva != 0 ? Math.Round(reserva * 100 / planReserva, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((reserva / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planReservaAcumulado, 2),
                RealAcumulado = Math.Round(reservaAcumulado, 2),
                PorcCumpAcumulado = planReservaAcumulado != 0 ? Math.Round(reservaAcumulado * 100 / planReservaAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((reservaAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });
            //utilidad despues de reserva de 30 %
            var planDespuesReserva = planDespuesContingencia - planReserva;
            var planDespuesReservaAcumulado = planDespuesContingenciaAcumulado - planReservaAcumulado;
            var despuesReserva = despuesContingencia - reserva;
            var despuesReservaAcumulado = despuesContingenciaAcumulado - reservaAcumulado;
            plan.Add(new PlanGIViewModel
            {
                Grupo = "Utilidad despues de la reserva del 30%",
                PlanMes = Math.Round(planDespuesReserva, 2),
                RealMes = Math.Round(despuesReserva, 2),
                PorcCumplimiento = planDespuesReserva != 0 ? Math.Round(despuesReserva * 100 / planDespuesReserva, 2, MidpointRounding.AwayFromZero) : 0,
                PorcRelacionIngresos = TotalIngresosEnMes != 0 ? Math.Round((despuesReserva / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotal = null,
                PlanAcumulado = Math.Round(planDespuesReservaAcumulado, 2),
                RealAcumulado = Math.Round(despuesReservaAcumulado, 2),
                PorcCumpAcumulado = planDespuesReservaAcumulado != 0 ? Math.Round(despuesReservaAcumulado * 100 / planDespuesReservaAcumulado, 2, MidpointRounding.AwayFromZero) : 0,
                PorcIngresosFuncionTotal = TotalIngresosAcumulados != 0 ? Math.Round((despuesReservaAcumulado / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                PorcGastosFuncionTotalAcumulado = null,
            });

            return plan;
        }
        public decimal ObtenerTotalPlanUtilidades(int year, int meses)
        {
            var planIngresos = ObtenerTotalPlanIngresos(year, meses);
            var planEgresos = ObtenerTotalPlanEgresos(year, meses);
            var total = planIngresos - planEgresos;
            return total;
        }
        public decimal ObtenerTotalUtilidades(int year, int meses)
        {
            var planIngresos = ObtenerTotalIngresos(year, meses);
            var planEgresos = ObtenerTotalEgresos(year, meses);
            var total = planIngresos - planEgresos;
            return total;
        }
    }
}
