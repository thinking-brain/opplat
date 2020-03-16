using System.Collections.Generic;

namespace FinanzasWebApi.Controllers
{
    public class ConfiguradorPlanViewModel
    {
        public string TipoPlan { get; set; }
        public List<dynamic> Items { get; set; }
    }
}