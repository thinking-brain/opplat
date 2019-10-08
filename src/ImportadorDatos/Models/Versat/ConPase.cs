using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class ConPase
    {
        public ConPase()
        {
            
        }

        public int Idpase { get; set; }
        public int Idcomprobante { get; set; }
        public int Idcuenta { get; set; }
        public decimal Importe { get; set; }
        public long Crc { get; set; }

        public virtual ConComprobante IdcomprobanteNavigation { get; set; }
        public virtual ConCuenta IdcuentaNavigation { get; set; }
    }
}
