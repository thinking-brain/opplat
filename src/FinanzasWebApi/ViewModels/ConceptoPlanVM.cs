using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.ViewModels
{
    public class ConceptoPlanVM
    {
        public int Id { get; set; }
        public string Concepto { get; set; }
      
        public ICollection<ConceptoCuentasVM> Cuentas { get; set; }


    }
}
