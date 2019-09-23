using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class GetNomDocumento
    {
        public static List<NomDocumentoVM> Get()
        {
            //Nomina Documento 
            HttpClient clientNDD = new HttpClient();
            List<NomDocumentoVM> nominaDoc = new List<NomDocumentoVM>();
            var resultNDD = clientNDD.GetAsync("http://127.0.0.1:5200/contabilidad/NomDocumento").Result;
            if (resultNDD.IsSuccessStatusCode)
            {
                nominaDoc = resultNDD.Content.ReadAsAsync<List<NomDocumentoVM>>().Result;
            }

           
            return nominaDoc;
        }
    }
}
