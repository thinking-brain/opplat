using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class ConApertura
    {
        public ConApertura()
        {
            ConCuenta = new HashSet<ConCuenta>();
        }

        public int Idapertura { get; set; }
        public int Idmascara { get; set; }
        public int? Idunidad { get; set; }
        public byte Tipo { get; set; }

        public virtual GenMascara IdmascaraNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
        public virtual ICollection<ConCuenta> ConCuenta { get; set; }
    }
}
