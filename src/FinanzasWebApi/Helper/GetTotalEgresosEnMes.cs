using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetTotalEgresosEnMes
    {
        IConfiguration _config { get; set; }

        public GetTotalEgresosEnMes(IConfiguration config)
        {
            _config = config;
        }

        public decimal GetReal(int year, int meses)
        {
            List<SubMayorCuentaVM> subMCuentas = GetSubMayorDeCuentas.Get(_config);
            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();
            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();
            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();
            //EGRESOS
            // Impuestos por las ventas
            var RealMes805 = dataInMes.Where(s => s.ClaveCuenta.Equals("805")).Sum(s => s.Importe);
            var RealMes810 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("810")).Sum(s => s.Importe);
            var RealMes822 = (dataInMes.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe));
            var RealMes835 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("835")).Sum(s => s.Importe);
            var RealMes836 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("836")).Sum(s => s.Importe);
            var RealMes845 = dataInMes.Where(s => s.ClaveCuenta.Equals("845")).Sum(s => s.Importe);
            var RealMes856 = dataInMes.Where(s => s.ClaveCuenta.StartsWith("856")).Sum(s => s.Importe);

            var TotalEgresosEnMes = RealMes805 + RealMes810 + RealMes822 + RealMes835 + RealMes836 + RealMes845 + RealMes856;


            return TotalEgresosEnMes;
        }
        public decimal GetPlan(int year, int meses)
        {
            List<PlanGIResultVM> planGI = GetPlanGI.Get(_config);

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));

            //EGRESOS
            var PlanMes805 = planesGI.SingleOrDefault(s => s.ElementoValor == "805") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "805")[meses] : 0M;
            var PlanMes810 = planesGI.SingleOrDefault(s => s.ElementoValor == "810") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "810")[meses] : 0M;
            var PlanMes822 = planesGI.SingleOrDefault(s => s.ElementoValor == "822") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "822")[meses] : 0M;
            var PlanMes835 = planesGI.SingleOrDefault(s => s.ElementoValor == "835") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "835")[meses] : 0M;
            var PlanMes836 = planesGI.SingleOrDefault(s => s.ElementoValor == "836") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "836")[meses] : 0M;
            var PlanMes845 = planesGI.SingleOrDefault(s => s.ElementoValor == "845") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "845")[meses] : 0M;
            var PlanMes856 = planesGI.SingleOrDefault(s => s.ElementoValor == "856") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "856")[meses] : 0M;

            decimal TotalPlanEgresosEnMes = PlanMes805 + PlanMes810 + PlanMes822 + PlanMes835 + PlanMes836 + PlanMes845 + PlanMes856;

            return TotalPlanEgresosEnMes;
        }
        public decimal GetRealAcumulado(int year, int meses)
        {
            List<SubMayorCuentaVM> subMCuentas = GetSubMayorDeCuentas.Get(_config);
            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = subMCuentas.ToList();
            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = subMCuentas.OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //EGRESOS
            var Acumulado805 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("805")).Sum(s => s.Importe);
            var Acumulado810 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("810")).Sum(s => s.Importe);
            var Acumulado822 = (dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe) + dataInMes.Where(s => s.ClaveCuenta.Equals("822")).Sum(s => s.Importe));
            var Acumulado835 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("835")).Sum(s => s.Importe);
            var Acumulado836 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.StartsWith("836")).Sum(s => s.Importe);
            var Acumulado845 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("845")).Sum(s => s.Importe);
            var Acumulado856 = dataInMesesAnteriores.Where(s => s.ClaveCuenta.Equals("856")).Sum(s => s.Importe);

            decimal TotalEgresosAcumulados = Acumulado805 + Acumulado810 + Acumulado822 + Acumulado835 + Acumulado836 + Acumulado845 + Acumulado856;
            return TotalEgresosAcumulados;
        }
        public decimal GetPlanAcumulado(int year, int meses)
        {
            List<PlanGIResultVM> planGI = GetPlanGI.Get(_config);

            //Tabla que contiene los palnes de Gastos e Ingresos
            var planesGI = planGI.Where(s => s.Año == Convert.ToString(year));
            //EGRESOS
            var PlanAcumulado805 = planesGI.SingleOrDefault(s => s.ElementoValor == "805") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "805").Acumulado(meses) : 0M;
            var PlanAcumulado810 = planesGI.SingleOrDefault(s => s.ElementoValor == "810") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "810").Acumulado(meses) : 0M;
            var PlanAcumulado822 = planesGI.SingleOrDefault(s => s.ElementoValor == "822") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "822").Acumulado(meses) : 0M;
            var PlanAcumulado835 = planesGI.SingleOrDefault(s => s.ElementoValor == "835") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "835").Acumulado(meses) : 0M;
            var PlanAcumulado836 = planesGI.SingleOrDefault(s => s.ElementoValor == "836") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "836").Acumulado(meses) : 0M;
            var PlanAcumulado845 = planesGI.SingleOrDefault(s => s.ElementoValor == "845") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "845").Acumulado(meses) : 0M;
            var PlanAcumulado856 = planesGI.SingleOrDefault(s => s.ElementoValor == "856") != null ? planesGI.SingleOrDefault(s => s.ElementoValor == "856").Acumulado(meses) : 0M;

            decimal TotalPlanEgresosAcumulados = PlanAcumulado805 + PlanAcumulado810 + PlanAcumulado822 + PlanAcumulado835 + PlanAcumulado836 + PlanAcumulado845 + PlanAcumulado856;

            return TotalPlanEgresosAcumulados;
        }
    }
}
