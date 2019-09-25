using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.ViewModels;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FinanzasWebApi.Helper
{
    public class GetPlanGI
    {
        public static List<PlanGIResultVM> Get(IConfiguration config)
        {
            //Plan IG
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient clientPGI = new HttpClient(handler);
            List<PlanGIResultVM> planGI = new List<PlanGIResultVM>();
            var url = config.GetValue<string>("ContabilidadApi") + "/PlanIG/PlanesIG";
            var resultPGI = clientPGI.GetAsync(url).Result;
            if (resultPGI.IsSuccessStatusCode)
            {
                planGI = resultPGI.Content.ReadAsAsync<List<PlanGIResultVM>>().Result;
            }

            return planGI;
        }
    }
}
