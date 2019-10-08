using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetMovimientoDeCuentaPeriodo
    {
        public static decimal Get(int year, int meses, string cuenta, IConfiguration config)
        {
            string fechaInicio = year+"-"+meses+"-"+1;
            string fechaFin = year+"-"+meses+"-"+31;
            //SUBMAYOR DE CUENTAS
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(handler);
            MovimientoCuentaPeriodoVM subMCuentas = new MovimientoCuentaPeriodoVM();
            var url = config.GetValue<string>("ContabilidadApi") + "/MovimientoDeCuentas" +"/"+cuenta+"/"+fechaInicio+"/"+fechaFin;
            var result = client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<MovimientoCuentaPeriodoVM>().Result;
            }


            return subMCuentas.Importe;
        }
    }
}
