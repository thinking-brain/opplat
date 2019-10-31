using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.ViewModels
{
    public class TotalIngresosViewModel
    {
        public decimal Real { get; set; }
        public decimal Plan { get; set; }
        public List<decimal> Reales{ get; set; }
        public List<decimal> Planes{ get; set; }

        public TotalIngresosViewModel()
        {
            Reales = new List<decimal>();
            Planes = new List<decimal>();
        }
    }
}
