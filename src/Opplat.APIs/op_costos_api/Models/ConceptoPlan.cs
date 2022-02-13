using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Models
{
    public class ConceptoPlan
    {
        public int Id { get; set; }
        public string Concepto { get; set; }
      
        public ICollection<ConceptoCuentas> Cuentas { get; set; }

        public ConceptoPlan()
        {
            this.Cuentas = new HashSet<ConceptoCuentas>();
        }
    }
}
