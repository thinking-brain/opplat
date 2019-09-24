using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetNomDocumento
    {
        public static List<NomDocumentoVM> Get(IConfiguration config)
        {
            //Nomina Documento 
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient clientNDD = new HttpClient(handler);
            List<NomDocumentoVM> nominaDoc = new List<NomDocumentoVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/NomDocumento";
            var resultNDD = clientNDD.GetAsync(url).Result;
            if (resultNDD.IsSuccessStatusCode)
            {
                nominaDoc = resultNDD.Content.ReadAsAsync<List<NomDocumentoVM>>().Result;
            }
            return nominaDoc;
        }
    }
}
