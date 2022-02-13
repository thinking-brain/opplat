using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenAperturaarea
    {
        public int Idapertura { get; set; }
        public int Idmascara { get; set; }
        public bool? Empresa { get; set; }

        public virtual GenMascara IdmascaraNavigation { get; set; }
    }
}
