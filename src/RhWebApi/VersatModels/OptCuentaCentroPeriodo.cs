using System;
using System.Collections.Generic;

namespace RhWebApi.VersatModels
{
    public partial class OptCuentaCentroPeriodo
    {
        public int Idunidad { get; set; }
        public int Idperiodo { get; set; }
        public int Idcuenta { get; set; }
        public int Idcentro { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Clavecuenta { get; set; }
        public string Clavecentro { get; set; }

        public virtual CosCentro IdcentroNavigation { get; set; }
        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
    }
}
