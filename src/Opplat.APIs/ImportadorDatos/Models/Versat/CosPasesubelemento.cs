using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class CosPasesubelemento
    {
        public int Idpase { get; set; }
        public int Idsubelemento { get; set; }

        public virtual CosPasecentro IdpaseNavigation { get; set; }
        public virtual CosSubelementogasto IdsubelementoNavigation { get; set; }
    }
}
