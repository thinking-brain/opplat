using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.VersatModels2
{
    public partial class NomAsientoGasto
    {
        public int Idasiento { get; set; }
        public int Idcentro { get; set; }
        public int Idsubelemento { get; set; }
        public decimal? NImporte { get; set; }
        public decimal? NImportemo { get; set; }

        public virtual NomAsiento IdasientoNavigation { get; set; }
        public virtual CosCentro IdcentroNavigation { get; set; }
        public virtual CosSubelementogasto IdsubelementoNavigation { get; set; }
    }
}
