using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.VersatModels
{
    public partial class OptCentroPeriodo
    {
        public int Idunidad { get; set; }
        public int Idperiodo { get; set; }
        public int Idcentro { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Clave { get; set; }

        public virtual CosCentro IdcentroNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
       
    }
}
