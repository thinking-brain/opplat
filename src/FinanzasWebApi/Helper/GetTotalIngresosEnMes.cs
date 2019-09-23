using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class GetTotalIngresosEnMes
    {
        public static decimal GetReal(int year, int meses)
        {
            List<SubMayorCuentaVM> subMCuentas = GetSubMayorDeCuentas.Get();
            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();
            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();
            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();
            //INGRESOS
            var RealMes912 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
            var RealMes913 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            var RealMes920 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;

            decimal TotalIngresosEnMes = RealMes912 + RealMes913 + RealMes920;

            return TotalIngresosEnMes;
        }
        public static decimal GetPlan(int year, int meses)
        {
            List<PlanGIResultVM> planGI = GetPlanGI.Get();

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //PLANES INGRESOS
            var PlanMes912 = planesGI.SingleOrDefault(s => s.ElementoValor == "912") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "912")[meses] : 0M;
            var PlanMes913 = planesGI.SingleOrDefault(s => s.ElementoValor == "913") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "913")[meses] : 0M;
            var PlanMes920 = planesGI.SingleOrDefault(s => s.ElementoValor == "920") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "920")[meses] : 0M;


            decimal TotalPlanIngresosEnMes = PlanMes912 + PlanMes913 + PlanMes920;

            return TotalPlanIngresosEnMes;
        }
        public static decimal GetRealAcumulado(int year, int meses)
        {
            List<SubMayorCuentaVM> subMCuentas = GetSubMayorDeCuentas.Get();
            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();
            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();
            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //INGRESOS
            var Acumulado912 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("912")).Sum(s => s.Importe) * -1;
            var Acumulado913 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("913")).Sum(s => s.Importe) * -1;
            var Acumulado920 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("920") || s.ClaveCuenta.StartsWith("921")).Sum(s => s.Importe) * -1;

            decimal TotalIngresosAcumulados = Acumulado912 + Acumulado913 + Acumulado920;

            return TotalIngresosAcumulados;
        }
        public static decimal GetPlanAcumulado(int year, int meses)
        {
            List<PlanGIResultVM> planGI = GetPlanGI.Get();

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //PLANES INGRESOS
            var PlanAcumulado912 = planesGI.SingleOrDefault(s => s.ElementoValor == "912") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "912").Acumulado(meses) : 0M;
            var PlanAcumulado913 = planesGI.SingleOrDefault(s => s.ElementoValor == "913") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "913").Acumulado(meses) : 0M;
            var PlanAcumulado920 = planesGI.SingleOrDefault(s => s.ElementoValor == "920") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "920").Acumulado(meses) : 0M;

            decimal TotalPlanIngresosAcumulado = PlanAcumulado912 + PlanAcumulado913 + PlanAcumulado920;

            return TotalPlanIngresosAcumulado;
        }


    }
}
