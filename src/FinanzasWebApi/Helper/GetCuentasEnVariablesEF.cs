using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetCuentasEnVariablesEF
    {
        public static List<string> Get(string concepto, IConfiguration config)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(handler);
            List<string> subMCuentas = new List<string>();
            var url = config.GetValue<string>("ContabilidadApi") + "/CuentasEnVariablesEF" + "/" + concepto;
            var result = client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<string>>().Result;
            }


            return subMCuentas;
        }
    }
}
