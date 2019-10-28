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
        public  ICollection<decimal> Reales{ get; set; }
        public  ICollection<decimal> Planes{ get; set; }

        public TotalIngresosViewModel()
        {
            this.Reales= new HashSet<decimal>();
            this.Planes= new HashSet<decimal>();
        }
    }
}
