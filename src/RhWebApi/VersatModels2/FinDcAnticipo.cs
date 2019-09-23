using System;
using System.Collections.Generic;

namespace RhWebApi.VersatModels2
{
    public partial class FinDcAnticipo
    {
        public int Iddetalleanticipo { get; set; }
        public int Iddcdoc { get; set; }
        public int Idctacont { get; set; }
        public decimal Importe { get; set; }
        public decimal Importemc { get; set; }

        public virtual ConCuenta IdctacontNavigation { get; set; }
    }
}
