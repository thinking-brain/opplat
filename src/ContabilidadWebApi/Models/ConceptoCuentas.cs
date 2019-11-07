using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Models
{
    public class ConceptoCuentas
    {
        public int ConceptoPlanId { get; set; }

        public virtual ConceptoPlan ConceptoPlan { get; set; }

        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }
    }
}
