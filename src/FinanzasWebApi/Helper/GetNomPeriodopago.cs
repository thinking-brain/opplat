using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetNomPeriodopago
    {
        public static List<NomPeriodopagoVM> Get(IConfiguration config)
        {
            //Nomina Periodo de Pago ED
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient clientPP = new HttpClient(handler);

            List<NomPeriodopagoVM> nominaPP = new List<NomPeriodopagoVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/NomPeriodopago";
            var resultPP = clientPP.GetAsync(url).Result;
            if (resultPP.IsSuccessStatusCode)
            {
                nominaPP = resultPP.Content.ReadAsAsync<List<NomPeriodopagoVM>>().Result;
            }
            return nominaPP;
        }
    }
}
