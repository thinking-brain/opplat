using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;

namespace FinanzasWebApi.Helper
{
    public class GetPlanGI
    {
        public static List<PlanGIResultVM> Get()
        {
            //Plan IG
            HttpClient clientPGI = new HttpClient();
            List<PlanGIResultVM> planGI = new List<PlanGIResultVM>();
            var resultPGI = clientPGI.GetAsync( "https://localhost:5001/contabilidad/PlanIG/PlanesIG").Result;
            if (resultPGI.IsSuccessStatusCode)
            {
                planGI = resultPGI.Content.ReadAsAsync<List<PlanGIResultVM>>().Result;
            }
           
            return planGI;
        }
    }
}
