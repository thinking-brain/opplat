using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetSubMayorDeCuentas
    {
        public static List<SubMayorCuentaVM> Get(IConfiguration config)
        {
            //SUBMAYOR DE CUENTAS
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(handler);
            List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/SubmayorDeCuentas";
            var result = client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
            }


            return subMCuentas;
        }
    }
}
