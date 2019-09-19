using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.ViewModels
{
    public class EjecucionViewModel
    {
        public string CentroCosto { get; set;}
        public DateTime Fecha { get; set;}
        public string Elemento { get; set;}
        public string Subelemento { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal MovimientoPeriodo { get; set; }
        public decimal SaldoActual { get; set; }
    }
}
