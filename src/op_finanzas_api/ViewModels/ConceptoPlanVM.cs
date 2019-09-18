using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_finanzas_api.ViewModels
{
    public class ConceptoPlanVM
    {
        public int Id { get; set; }
        public string Concepto { get; set; }
      
        public ICollection<ConceptoCuentasVM> Cuentas { get; set; }


    }
}
