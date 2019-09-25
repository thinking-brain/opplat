using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetNomDocumentoDetallePago
    {
        public static List<NomDocumentoDetallePagoVM> Get(IConfiguration config)
        {
            //Nomina Detalle Doc
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient clientDetDoc = new HttpClient(handler);
            List<NomDocumentoDetallePagoVM> nominaDetDoc = new List<NomDocumentoDetallePagoVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/NomDocumentoDetallePago";
            var resultDetDoc = clientDetDoc.GetAsync(url).Result;
            if (resultDetDoc.IsSuccessStatusCode)
            {
                nominaDetDoc = resultDetDoc.Content.ReadAsAsync<List<NomDocumentoDetallePagoVM>>().Result;
            }

            return nominaDetDoc;
        }
    }
}
