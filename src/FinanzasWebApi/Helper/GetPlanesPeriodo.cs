using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetPlanesPeriodo
    {
        public static List<DetallePlanIGVM> Get(int year, IConfiguration config)
        {
            string año = Convert.ToString(year);
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(handler);
            List<DetallePlanIGVM> subMCuentas = new List<DetallePlanIGVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/Planes" + "/" + año;
            var result = client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<DetallePlanIGVM>>().Result;
            }
            return subMCuentas;
        }
    }
}
