using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class GetNomPeriodopago
    {
        public static List<NomPeriodopagoVM> Get()
        {
            //Nomina Periodo de Pago ED
            HttpClient clientPP = new HttpClient();
            List<NomPeriodopagoVM> nominaPP = new List<NomPeriodopagoVM>();
            var resultPP = clientPP.GetAsync("https://localhost:5001/contabilidad/NomPeriodopago").Result;
            if (resultPP.IsSuccessStatusCode)
            {
                nominaPP = resultPP.Content.ReadAsAsync<List<NomPeriodopagoVM>>().Result;
            }
            return nominaPP;
        }
    }
}
