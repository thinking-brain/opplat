using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class ConComprobante
    {
        public ConComprobante()
        {

        }

        public int Idcomprobante { get; set; }
        public int Idunidad { get; set; }
        public string Descripcion { get; set; }
        public string Sumaclave { get; set; }
        public decimal? Debito { get; set; }
        public decimal? Credito { get; set; }
        public virtual ConComprobanteoperacion ConComprobanteoperacion { get; set; }
        public virtual ICollection<ConPase> ConPase { get; set; }
    }
}
