using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class ObtenerPlanGI_Context
    {
        
        public List<PlanGIViewModel> ObtenerIngresos(int year, int meses)
        {
             //Plan IG
            HttpClient clientPGI = new HttpClient();
            List<PlanGIResultVM> planGI = new List<PlanGIResultVM>();
            var resultPGI = clientPGI.GetAsync("http://127.0.0.1:5200/contabilidad/PlanIG/PlanesIG").Result;
            if (resultPGI.IsSuccessStatusCode)
            {
                planGI = resultPGI.Content.ReadAsAsync<List<PlanGIResultVM>>().Result;
            }
          
            //SUBMAYOR DE CUENTAS
            HttpClient client = new HttpClient();
            List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
            var result = client.GetAsync("http://127.0.0.1:5200/contabilidad/SubmayorDeCuentas").Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
            }
           

            var plan = new List<PlanGIViewModel>();

            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
           
            var data = subMCuentas.ToList();

            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();

            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();

            //Seleccionando todos los movimientos que esten entre el comienzo del año y sean menores e igual al mes consultado
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //Seleccionando los movimientos del mes siguiente al mes consultado (Para los pagos de Utilidad?????)
            var dataInMesSiguiente = data.Where(s => s.Mes == (meses + 1)).ToList();

            //Obteniendo listado de Elementos del Plan de Gastos e Ingresos
            var modelo = DatosPlanGI.Datos();
            //INGRESOS
            // Ventas por servicios en moneda nacional
            var RealMes912 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
            var PlanMes912 = planesGI.SingleOrDefault(s => s.ElementoValor == "912") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "912")[meses] : 0M;
            var PlanAcumulado912 = planesGI.SingleOrDefault(s => s.ElementoValor == "912") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "912").Acumulado(meses) : 0M;
            var Acumulado912 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
            // Ventas por servicios en CUC
            var RealMes913 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            var PlanMes913 = planesGI.SingleOrDefault(s => s.ElementoValor == "913") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "913")[meses] : 0M;
            var PlanAcumulado913 = planesGI.SingleOrDefault(s => s.ElementoValor == "913") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "913").Acumulado(meses) : 0M;
            var Acumulado913 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            // Ingresos financieros
            var RealMes920 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;
            var PlanMes920 = planesGI.SingleOrDefault(s => s.ElementoValor == "920") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "920")[meses] : 0M;
            var PlanAcumulado920 = planesGI.SingleOrDefault(s => s.ElementoValor == "920") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "920").Acumulado(meses) : 0M;
            var Acumulado920 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;

            var TotalIngresosEnMes = RealMes912 + RealMes913 + RealMes920;



            foreach (var item in modelo.ToList())
            {
                //INGRESOS               
                // Ventas por servicios en moneda nacional
                if (item.Valor == 912)
                {
                    var PlanMes = PlanMes912;
                    var PlanAcumulado = PlanAcumulado912;
                    var RealMes = RealMes912;
                    var RealAcumulado = Acumulado912;
                    var TotalIngresos = TotalIngresosEnMes;

                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalIngresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalIngresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //% Cumplimiento
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        ////Total en el Grupo
                        //

                    };
                    plan.Add(cost);
                }
                // Ventas por servicios en CUC
                if (item.Valor == 913)
                {
                    var PlanMes = PlanMes913;
                    var PlanAcumulado = PlanAcumulado913;
                    var RealMes = RealMes913;
                    var RealAcumulado = Acumulado913;
                    var TotalIngresos = TotalIngresosEnMes;


                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalIngresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalIngresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,

                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Ingresos financieros
                if (item.Valor == 920)
                {
                    var PlanMes = PlanMes920;
                    var PlanAcumulado = PlanAcumulado920;
                    var RealMes = RealMes920;
                    var RealAcumulado = Acumulado920;
                    var TotalIngresos = TotalIngresosEnMes;

                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalIngresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalIngresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
            }

            return plan;
        }

        public List<PlanGIViewModel> ObtenerEgresos(int year, int meses)
        {
              //Plan IG
            HttpClient clientPGI = new HttpClient();
            List<PlanGIResultVM> planGI = new List<PlanGIResultVM>();
            var resultPGI = clientPGI.GetAsync("http://127.0.0.1:5200/contabilidad/PlanIG/PlanesIG").Result;
            if (resultPGI.IsSuccessStatusCode)
            {
                planGI = resultPGI.Content.ReadAsAsync<List<PlanGIResultVM>>().Result;
            }
          
            //SUBMAYOR DE CUENTAS
            HttpClient client = new HttpClient();
            List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
            var result = client.GetAsync("http://127.0.0.1:5200/contabilidad/SubmayorDeCuentas").Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
            }
           


            var plan = new List<PlanGIViewModel>();

            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();

            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();

            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();

            //Seleccionando todos los movimientos que esten entre el comienzo del año y sean menores e igual al mes consultado
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //Seleccionando los movimientos del mes siguiente al mes consultado (Para los pagos de Utilidad?????)
            var dataInMesSiguiente = data.Where(s => s.Mes == (meses + 1)).ToList();

            //Obteniendo listado de Elementos del Plan de Gastos e Ingresos
            var modelo = DatosPlanGI.Datos();

            //EGRESOS
            // Impuestos por las ventas
            var RealMes805 = dataInMes.Where(s => s.ClaveCuenta.Equals("805")).Sum(s => s.Importe);
            var Acumulado805 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("805")).Sum(s => s.Importe);
            var PlanMes805 = planesGI.SingleOrDefault(s => s.ElementoValor == "805") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "805")[meses] : 0M;
            var PlanAcumulado805 = planesGI.SingleOrDefault(s => s.ElementoValor == "805") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "805").Acumulado(meses) : 0M;
            // Costo de ventas
            var RealMes810 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("810")).Sum(s => s.Importe);
            var PlanMes810 = planesGI.SingleOrDefault(s => s.ElementoValor == "810") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "810")[meses] : 0M;
            var PlanAcumulado810 = planesGI.SingleOrDefault(s => s.ElementoValor == "810") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "810").Acumulado(meses) : 0M;
            var Acumulado810 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("810")).Sum(s => s.Importe);
            // Gastos generales de Administración
            var RealMes822 = (dataInMes.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe));
            var PlanMes822 = planesGI.SingleOrDefault(s => s.ElementoValor == "822") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "822")[meses] : 0M;
            var PlanAcumulado822 = planesGI.SingleOrDefault(s => s.ElementoValor == "822") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "822").Acumulado(meses) : 0M;
            var Acumulado822 = (dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe) + dataInMes.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe));
            // Gastos financieros en MN
            var RealMes835 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("835")).Sum(s => s.Importe);
            var PlanMes835 = planesGI.SingleOrDefault(s => s.ElementoValor == "835") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "835")[meses] : 0M;
            var PlanAcumulado835 = planesGI.SingleOrDefault(s => s.ElementoValor == "835") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "835").Acumulado(meses) : 0M;
            var Acumulado835 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("835")).Sum(s => s.Importe);
            // Gastos financieros en MLC
            var RealMes836 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("836")).Sum(s => s.Importe);
            var PlanMes836 = planesGI.SingleOrDefault(s => s.ElementoValor == "836") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "836")[meses] : 0M;
            var PlanAcumulado836 = planesGI.SingleOrDefault(s => s.ElementoValor == "836") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "836").Acumulado(meses) : 0M;
            var Acumulado836 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("836")).Sum(s => s.Importe);
            // Gastos por perdida
            var RealMes845 = dataInMes.Where(s => s.ClaveCuenta.Equals("845")).Sum(s => s.Importe);
            var PlanMes845 = planesGI.SingleOrDefault(s => s.ElementoValor == "845") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "845")[meses] : 0M;
            var PlanAcumulado845 = planesGI.SingleOrDefault(s => s.ElementoValor == "845") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "845").Acumulado(meses) : 0M;
            var Acumulado845 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("845")).Sum(s => s.Importe);
            // Otros impuestos tasas y contribuciones
            var RealMes856 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("856")).Sum(s => s.Importe);
            var PlanMes856 = planesGI.SingleOrDefault(s => s.ElementoValor == "856") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "856")[meses] : 0M;
            var PlanAcumulado856 = planesGI.SingleOrDefault(s => s.ElementoValor == "856") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "856").Acumulado(meses) : 0M;
            var Acumulado856 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("856")).Sum(s => s.Importe);

            var TotalEgresosEnMes = RealMes805 + RealMes810 + RealMes822 + RealMes835 + RealMes836 + RealMes845 + RealMes856;

            //Total de ingresos Acumulado
            var Acumulado912 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
            var Acumulado913 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            var Acumulado920 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;
            var TotalIngresosAcumulados = Acumulado912 + Acumulado913 + Acumulado920;

            //Total de los Egresos Acumulado
            var TotalEgresosAcumuados = Acumulado805 + Acumulado810 + Acumulado822 + Acumulado835 + Acumulado836 + Acumulado845 + Acumulado856;

            foreach (var item in modelo.ToList())
            {
                //EGRESOS
                // Impuestos por las ventas
                if (item.Valor == 805)
                {
                    var PlanMes = PlanMes805;
                    var PlanAcumulado = PlanAcumulado805;
                    var RealMes = RealMes805;
                    var RealAcumulado = Acumulado805;
                    var TotalEgresos = TotalEgresosEnMes;


                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Costo de ventas
                if (item.Valor == 810)
                {
                    var PlanMes = PlanMes810;
                    var PlanAcumulado = PlanAcumulado810;
                    var RealMes = RealMes810;
                    var RealAcumulado = Acumulado810;
                    var TotalEgresos = TotalEgresosEnMes;


                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Gastos generales de Administración
                if (item.Valor == 822)
                {
                    var PlanMes = PlanMes822;
                    var PlanAcumulado = PlanAcumulado822;
                    var RealMes = RealMes822;
                    var RealAcumulado = Acumulado822;
                    var TotalEgresos = TotalEgresosEnMes;


                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Gastos financieros en MN
                if (item.Valor == 835)
                {
                    var PlanMes = PlanMes835;
                    var PlanAcumulado = PlanAcumulado835;
                    var RealMes = RealMes835;
                    var RealAcumulado = Acumulado835;
                    var TotalEgresos = TotalEgresosEnMes;

                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Gastos financieros en MLC
                if (item.Valor == 836)
                {
                    var PlanMes = PlanMes836;
                    var PlanAcumulado = PlanAcumulado836;
                    var RealMes = RealMes836;
                    var RealAcumulado = Acumulado836;
                    var TotalEgresos = TotalEgresosEnMes;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Gastos por perdida
                if (item.Valor == 845)
                {
                    var PlanMes = PlanMes845;
                    var PlanAcumulado = PlanAcumulado845;
                    var RealMes = RealMes845;
                    var RealAcumulado = Acumulado845;
                    var TotalEgresos = TotalEgresosEnMes;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
                // Otros impuestos tasas y contribuciones
                if (item.Valor == 856)
                {
                    var PlanMes = PlanMes856;
                    var PlanAcumulado = PlanAcumulado856;
                    var RealMes = RealMes856;
                    var RealAcumulado = Acumulado856;
                    var TotalEgresos = TotalEgresosEnMes;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = (RealMes > 0 && TotalEgresos > 0) ? Math.Round((RealMes / TotalEgresos) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = (RealMes > 0 && TotalIngresosAcumulados > 0) ? Math.Round((RealMes / TotalIngresosAcumulados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = (RealMes > 0 && TotalEgresosAcumuados > 0) ? Math.Round((RealMes / TotalEgresosAcumuados) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //Total en el Grupo
                        
                    };
                    plan.Add(cost);

                }
            }
            return plan;
        }

        public List<PlanGIViewModel> ObtenerUtilidades(int year, int meses)
        {
            //Plan IG
            HttpClient clientPGI = new HttpClient();
            List<PlanGIResultVM> planGI = new List<PlanGIResultVM>();
            var resultPGI = clientPGI.GetAsync( "http://127.0.0.1:5200/contabilidad/PlanIG/PlanesIG").Result;
            if (resultPGI.IsSuccessStatusCode)
            {
                planGI = resultPGI.Content.ReadAsAsync<List<PlanGIResultVM>>().Result;
            }
          
            //SUBMAYOR DE CUENTAS
            HttpClient client = new HttpClient();
            List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
            var result = client.GetAsync("http://127.0.0.1:5200/contabilidad/SubmayorDeCuentas").Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
            }

            //Nomina Documento 
            HttpClient clientNDD = new HttpClient();
            List<NomDocumentoVM> nominaDoc = new List<NomDocumentoVM>();
            var resultNDD = clientNDD.GetAsync("http://127.0.0.1:5200/contabilidad/NomDocumento").Result;
            if (resultNDD.IsSuccessStatusCode)
            {
                nominaDoc = resultNDD.Content.ReadAsAsync<List<NomDocumentoVM>>().Result;
            }

            //Nomina Periodo de Pago ED
            HttpClient clientPP = new HttpClient();
            List<NomPeriodopagoVM> nominaPP = new List<NomPeriodopagoVM>();
            var resultPP = clientPP.GetAsync("http://127.0.0.1:5200/contabilidad/NomPeriodopago").Result;
            if (resultPP.IsSuccessStatusCode)
            {
                nominaPP = resultPP.Content.ReadAsAsync<List<NomPeriodopagoVM>>().Result;
            }

             //Nomina Detalle Doc
            HttpClient clientDetDoc = new HttpClient();
            List<NomDocumentoDetallePagoVM> nominaDetDoc = new List<NomDocumentoDetallePagoVM>();
            var resultDetDoc = clientDetDoc.GetAsync("http://127.0.0.1:5200/contabilidad/NomDocumentoDetallePago").Result;
            if (resultDetDoc.IsSuccessStatusCode)
            {
                nominaDetDoc = resultDetDoc.Content.ReadAsAsync<List<NomDocumentoDetallePagoVM>>().Result;
            }

         
      
            var plan = new List<PlanGIViewModel>();

            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();

            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();

            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();

            //Seleccionando todos los movimientos que esten entre el comienzo del año y sean menores e igual al mes consultado
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //Seleccionando todos los movimientos en la tabla de detalles de documentos que esten entre el comienzo del año y sean menores e igual al mes consultado
            var dataDocInMesesAnteriores = nominaDetDoc.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //Seleccionando los movimientos del mes siguiente al mes consultado (Para los pagos de Utilidad?????)
            var dataInMesSiguiente = data.Where(s => s.Mes == (meses + 1)).ToList();

            //Obteniendo listado de Elementos del Plan de Gastos e Ingresos
            var modelo = DatosPlanGI.Datos();
            //INGRESOS
            // Ventas por servicios en moneda nacional
            var RealMes912 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
           
            // Ventas por servicios en CUC
            var RealMes913 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            
            // Ingresos financieros
            var RealMes920 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;

            var TotalIngresosEnMes = RealMes912 + RealMes913 + RealMes920;

            //EGRESOS
            // Impuestos por las ventas
            var RealMes805 = dataInMes.Where(s => s.ClaveCuenta.Equals("805")).Sum(s => s.Importe);
            // Costo de ventas
            var RealMes810 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("810")).Sum(s => s.Importe);
            // Gastos generales de Administración
            var RealMes822 = (dataInMes.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe));
            // Gastos financieros en MN
            var RealMes835 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("835")).Sum(s => s.Importe);
            // Gastos financieros en MLC
            var RealMes836 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("836")).Sum(s => s.Importe);
            // Gastos por perdida
            var RealMes845 = dataInMes.Where(s => s.ClaveCuenta.Equals("845")).Sum(s => s.Importe);
            // Otros impuestos tasas y contribuciones
            var RealMes856 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("856")).Sum(s => s.Importe);

            var TotalEgresosEnMes = RealMes805 + RealMes810 + RealMes822 + RealMes835 + RealMes836 + RealMes845 + RealMes856;

            //UTILIDAD
            var RealMesUtilidad = TotalIngresosEnMes - TotalEgresosEnMes;

            // Pago a cargo de la utilidad
            decimal RealMes693 = 0M;
            decimal AcumuladoReal693 = 0M;

            if (meses <= 2)
            {
                var periodoContable = nominaPP.SingleOrDefault(s => s.PeriodoIni.Value.Year == year && s.PeriodoIni.Value.Month == (meses + 1));
                RealMes693 = RealMes693 + nominaDetDoc.Where(s => s.IdPeriodo == periodoContable.Idperiodopago && s.Idcuenta == 1571).Sum(s => s.NCobrar).Value;
                AcumuladoReal693 = AcumuladoReal693 + dataDocInMesesAnteriores.Where(s => s.Idcuenta == 1571).Sum(s => s.NCobrar).Value;
            }
            if (meses >= 3)
            {
                var periodoContable = nominaPP.SingleOrDefault(s => s.PeriodoIni.Value.Year == year && s.PeriodoIni.Value.Month == (meses + 1));
                RealMes693 = RealMes693 + nominaDetDoc.Where(s => s.IdPeriodo == periodoContable.Idperiodopago && s.Idcuenta == 1571).Sum(s => s.NCobrar).Value;
                AcumuladoReal693 = AcumuladoReal693 + dataDocInMesesAnteriores.Where(s => s.Idcuenta == 1571).Sum(s => s.NCobrar).Value;
            
            }


            var PlanMes693 = planesGI.SingleOrDefault(s => s.ElementoValor == "1001") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1001")[meses] : 0M;
            var PlanAcumulado693 = planesGI.SingleOrDefault(s => s.ElementoValor == "1001") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1001").Acumulado(meses) : 0M;
            
            
            // Utilidad despues de pagos de anticipos
            var RealMesUtilidadDespPago = RealMesUtilidad - RealMes693;
            var PlanMesUtilidadDespPago = planesGI.SingleOrDefault(s => s.ElementoValor == "1002") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1002")[meses] : 0M;
            var PlanAcumuladoUtilidadDespPago = planesGI.SingleOrDefault(s => s.ElementoValor == "1002") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1002").Acumulado(meses) : 0M;
            

            //Reserva de contingencia del 2% al 10%
            var RealMes2Porc = TotalEgresosEnMes * 0.16667M;
            var PlanMes2Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1003") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1003")[meses] : 0M;
            var PlanAcumulado2Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1003") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1003").Acumulado(meses) : 0M;
            
            // Utilidad libre despues de la reserva
            var PlanMesUtilidadDesp2Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1004") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1004")[meses] : 0M;
            var PlanAcumuladoUtilidadDesp2Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1004") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1004").Acumulado(meses) : 0M;
            
            // Reserva de contingencia 30%
            var RealMes30Porc = TotalEgresosEnMes * 2.5M;
            var PlanMes30Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1005") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1005")[meses] : 0M;
            var PlanAcumulado30Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1005") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1005").Acumulado(meses) : 0M;
            
            // Utilidad despues de la reserva del 30%
            var PlanMesUtilidadDesp30Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1006") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1006")[meses] : 0M;
            var PlanAcumuladoUtilidadDesp30Porc = planesGI.SingleOrDefault(s => s.ElementoValor == "1006") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "1006").Acumulado(meses) : 0M;

            foreach (var item in modelo.ToList())
            {

               
                // Pago a cargo de la utilidad
                if (item.Valor == 1001)
                {
                    var PlanMes = PlanMes693;
                    var PlanAcumulado = PlanAcumulado693;
                    var RealMes = RealMes693;
                    var RealAcumulado = AcumuladoReal693;
                    var TotalEgresos = 0;


                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);

                }
                // Utilidad despues de pagos de anticipos
                if (item.Valor == 1002)
                {
                    var PlanMes = PlanMesUtilidadDespPago;
                    var PlanAcumulado = PlanAcumuladoUtilidadDespPago;
                    var RealMes = RealMesUtilidadDespPago;
                    var RealAcumulado = 0;
                    var TotalEgresos = 0;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);

                }
                //Reserva de contingencia del 2% al 10%
                if (item.Valor == 1003)
                {
                    var PlanMes = PlanMes2Porc;
                    var PlanAcumulado = PlanAcumulado2Porc;
                    var RealMes = Math.Round(RealMes2Porc, 2);
                    var RealAcumulado = Math.Round(TotalEgresosEnMes * (0.16667M * meses),2,MidpointRounding.AwayFromZero);
                    var TotalEgresos = 0;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);

                }
                // Utilidad libre despues de la reserva
                if (item.Valor == 1004)
                {
                    var PlanMes = PlanMesUtilidadDesp2Porc;
                    var PlanAcumulado = PlanAcumuladoUtilidadDesp2Porc;
                    var RealMes = Math.Round(RealMesUtilidadDespPago - RealMes2Porc, 2);
                    var RealAcumulado = 0;
                    var TotalEgresos = 0;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);

                }
                // Reserva de contingencia 30%
                if (item.Valor == 1005)
                {
                    var PlanMes = PlanMes30Porc;
                    var PlanAcumulado = PlanAcumulado30Porc;
                    var RealMes = RealMes30Porc;
                    var RealAcumulado = 0;
                    var TotalEgresos = 0;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);

                }
                // Utilidad despues de la reserva del 30%
                if (item.Valor == 1006)
                {
                    var PlanMes = PlanMesUtilidadDesp30Porc;
                    var PlanAcumulado = PlanAcumuladoUtilidadDesp30Porc;
                    var RealMes = Math.Round((RealMesUtilidadDespPago - RealMes2Porc) - RealMes30Porc, 2);
                    var RealAcumulado = 0;
                    var TotalEgresos = 0;
                    var cost = new PlanGIViewModel
                    {
                        //Grupo
                        Grupo = item.Dato.ToString(),
                        //Plan Mensual
                        PlanMes = PlanMes,
                        //Real del Mes
                        RealMes = RealMes,

                        // % de Cumplimiento
                        PorcCumplimiento = (RealMes > 0 && PlanMes > 0) ? Math.Round((RealMes / PlanMes) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos
                        PorcRelacionIngresos = (TotalEgresos > 0 && RealMes > 0) ? Math.Round((RealMes / (TotalEgresos)) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        //% de los gastos en función del total
                        PorcGastosFuncionTotal = 0M,
                        //Plan Acumulado
                        PlanAcumulado = PlanAcumulado,
                        //Real Acumulado
                        RealAcumulado = RealAcumulado,
                        //
                        PorcCumpAcumulado = (RealAcumulado > 0 && PlanAcumulado > 0) ? Math.Round((RealAcumulado / PlanAcumulado) * 100, 2, MidpointRounding.AwayFromZero) : 0,
                        // % en relación a ingresos 
                        PorcIngresosFuncionTotal = 0M,
                        //% de los gastos en función del total 
                        PorcGastosFuncionTotalAcumulado = 0M,
                        //Total en el Grupo
                       
                    };
                    plan.Add(cost);


                }

            }
            return plan;
        }
    }

}
