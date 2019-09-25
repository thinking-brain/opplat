using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class GetNomDocumentoDetallePago
    {
        public static List<NomDocumentoDetallePagoVM> Get()
        {
           
             //Nomina Detalle Doc
            HttpClient clientDetDoc = new HttpClient();
            List<NomDocumentoDetallePagoVM> nominaDetDoc = new List<NomDocumentoDetallePagoVM>();
            var resultDetDoc = clientDetDoc.GetAsync("https://localhost:5001/contabilidad/NomDocumentoDetallePago").Result;
            if (resultDetDoc.IsSuccessStatusCode)
            {
                nominaDetDoc = resultDetDoc.Content.ReadAsAsync<List<NomDocumentoDetallePagoVM>>().Result;
            }
           
            return nominaDetDoc;
        }
    }
}
