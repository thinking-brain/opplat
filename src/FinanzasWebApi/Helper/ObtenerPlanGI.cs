using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class ObtenerPlanGI
    {
        IConfiguration _config { get; set; }

        public ObtenerPlanGI(IConfiguration config)
        {
            _config = config;
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
            var cta = "693";
            decimal Cuenta693 = GetMovimientoDeCuentaPeriodo.Get(year, meses, cta, _config);
            decimal ImporteAcumulado = GetMovimientoDeCuentaPeriodoAcumulado.Get(year, meses, cta, _config);
            foreach (var item in modelo.Where(s => s.Tipo.Equals("Utilidad")))
            {
                var cost = new PlanGIViewModel
                {
                    //Grupo
                    Grupo = item.Dato.ToString(),
                    //Plan Mensual
                    PlanMes = 0,
                    //Real del Mes
                    RealMes = 0,

                    // % de Cumplimiento
                    PorcCumplimiento = 0 > 0 ? Math.Round((0m / 100) * 100, 2, MidpointRounding.AwayFromZero) : 0,

                    // % en relación a ingresos
                    PorcRelacionIngresos = TotalIngresosEnMes > 0 ? Math.Round((0 / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,

                    //% de los gastos en función del total
                    PorcGastosFuncionTotal = null,
                    //Plan Acumulado
                    PlanAcumulado = 0,
                    //Real Acumulado
                    RealAcumulado = 0,
                    //
                    PorcCumpAcumulado = 0 > 0 ? Math.Round((0m / 100) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                    // % en relación a ingresos 
                    PorcIngresosFuncionTotal = TotalIngresosAcumulados > 0 ? Math.Round((0 / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                    //% de los gastos en función del total 
                    PorcGastosFuncionTotalAcumulado = null,
                    //Total en el Grupo

                };
                plan.Add(cost);
            }

            return plan;
        }
    }
}
