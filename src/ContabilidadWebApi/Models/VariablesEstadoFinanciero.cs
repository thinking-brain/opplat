using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Models
{
    public class VariablesEstadoFinanciero
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<VariablesEstadoFinancieroCuentas> Cuentas { get; set; }

        public VariablesEstadoFinanciero()
        {
            this.Cuentas = new HashSet<VariablesEstadoFinancieroCuentas>();
        }
    }
}
