using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.ViewModels
{
    public class CostosViewModel
    {
        public string Grupo { get; set; }
        public string Elementos { get; set; }
        public string SubElementos { get; set; }
        public decimal PlanMes { get; set; }
        public decimal RealMes { get; set; }
        public decimal PorcEjec { get; set; }
        public decimal PlanAcumulado { get; set; }
        public decimal RealAcumulado { get; set; }
        public decimal PorcRealEjec { get; set; }

    }
}
