using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.VersatModels
{
    public partial class OptCuentaPeriodoOK
    {
        public int Idpasecuenta { get; set; }
        public int Idunidad { get; set; }
        public int Idperiodo { get; set; }
        public int Idcuenta { get; set; }
        public decimal Importe { get; set; }
        public string Clavecuenta { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
    }
}
