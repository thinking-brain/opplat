using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Models
{
    public class VariablesEstadoFinancieroCuentas
    {
        public int VariablesEstadoFinancieroId { get; set; }

        public virtual VariablesEstadoFinanciero VariablesEstadoFinanciero { get; set; }

        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }
    }
}
