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
                string concepto = item.Dato.ToString();
                var planEnMes = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto))[meses] : 0M;
                var planAcum = planes.SingleOrDefault(s => s.Concepto.Equals(concepto)) != null ? planes.SingleOrDefault(s => s.Concepto.Equals(concepto)).Acumulado(meses) : 0M;

                // Pago a cargo de la utilidad
                if (item.Valor == 1001)
                {
                    var PlanMes = planEnMes;
                    var PlanAcumulado = planAcum;
                    var RealMes = Cuenta693;
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
                        PorcRelacionIngresos = TotalIngresosEnMes > 0 ? Math.Round((RealMes / (TotalIngresosEnMes)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,

                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = null,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = PlanAcumulado > 0 ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = TotalIngresosAcumulados > 0 ? Math.Round((RealMes / (TotalIngresosAcumulados)) * 100, 2, MidpointRounding.AwayFromZero) : 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = null,
                        //Total en el Grupo

                    };
                    plan.Add(cost);

                }
            }

            return plan;
        }
    }
}
