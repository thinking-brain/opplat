using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.ViewModels
{
    public class CostColumnReportApiVM
    {
        public string Titulo { get; set; }
        public string Fecha { get; set; }
        public string[] Headers { get; set; }
        public Dictionary<string, Dictionary<string, CostosViewModel>> Valores { get; set; }
        public Dictionary<string, List<decimal>> Parciales { get; set; }
        public List<decimal> General { get; set; }
    }
}
