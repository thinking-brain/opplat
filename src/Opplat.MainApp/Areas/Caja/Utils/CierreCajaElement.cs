using Commons.Core.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opplat.MainApp.Areas.Caja.Utils
{
    public class CierreCajaElement : ICierreContableElement
    {
        public string Descripcion { get ; set ; }
        public decimal Importe { get ; set ; }
        public int Factor { get; set ; }
        public IEnumerable<ICierreContableElement> SubElements { get; set; }
    }
}
