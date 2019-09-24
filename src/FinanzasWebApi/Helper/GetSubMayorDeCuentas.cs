using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;


namespace FinanzasWebApi.Helper
{
    public class GetSubMayorDeCuentas
    {
        public static List<SubMayorCuentaVM> Get()
        {
             //SUBMAYOR DE CUENTAS
            HttpClient client = new HttpClient();
            List<SubMayorCuentaVM> subMCuentas = new List<SubMayorCuentaVM>();
            var result = client.GetAsync("https://localhost:5001/contabilidad/SubmayorDeCuentas").Result;
            if (result.IsSuccessStatusCode)
            {
                subMCuentas = result.Content.ReadAsAsync<List<SubMayorCuentaVM>>().Result;
            }

           
            return subMCuentas;
        }
    }
}
