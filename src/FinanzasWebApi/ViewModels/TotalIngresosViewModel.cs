using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.ViewModels
{
    public class TotalIngresosViewModel
    {
        public decimal Real
        {
            get
            {
                return Reales.Sum(c => c);
            }
        }
        public decimal Plan
        {
            get
            {
                return Planes.Sum(p => p);
            }
        }
        public List<decimal> Reales { get; set; }
        public List<decimal> Planes { get; set; }

        public TotalIngresosViewModel()
        {
            Reales = new List<decimal>();
            Planes = new List<decimal>();
        }
    }
}
