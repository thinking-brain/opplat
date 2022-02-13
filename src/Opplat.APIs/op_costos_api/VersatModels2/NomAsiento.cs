using System;
using System.Collections.Generic;

namespace op_costos_api.VersatModels2
{
    public partial class NomAsiento
    {
      

        public int Idasiento { get; set; }
        public int Iddocumento { get; set; }
        public int Idcuenta { get; set; }
        public int Idcriterio { get; set; }
        public int? Idpase { get; set; }
        public decimal? NImporte { get; set; }
        public int? Idmoneda { get; set; }
        public int? Idtasa { get; set; }
        public decimal? NImportemo { get; set; }
        public int? IdtasaAnx { get; set; }
        public decimal? NImporteAnx { get; set; }
        public bool? Asientogasto { get; set; }
        public bool? Asientonxp { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual NomDocumento IddocumentoNavigation { get; set; }
        public virtual GenMoneda IdmonedaNavigation { get; set; }
        public virtual ICollection<NomAsientoGasto> NomAsientoGasto { get; set; }
    }
}
