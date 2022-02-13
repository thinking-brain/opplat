using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class CosPasecentro
    {
        public CosPasecentro()
        {

        }

        public int Idpase { get; set; }
        public decimal Importe { get; set; }
        public int Idregistro { get; set; }
        public int Idcentro { get; set; }

        public virtual CosCentro IdcentroNavigation { get; set; }
        public virtual CosRegistrogasto IdregistroNavigation { get; set; }
        public virtual CosPasesubelemento CosPasesubelemento { get; set; }
    }
}
