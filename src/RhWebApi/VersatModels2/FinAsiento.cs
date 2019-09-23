using System;
using System.Collections.Generic;

namespace RhWebApi.VersatModels2
{
    public partial class FinAsiento
    {
       

        public int Idasiento { get; set; }
        public int Idoperacion { get; set; }
        public int Idcriterio { get; set; }
        public decimal Importe { get; set; }
        public int? Idpase { get; set; }
        public bool Documento { get; set; }
        public int Variacion { get; set; }
        public int? Idtasa { get; set; }
        public int Idmoneda { get; set; }
        public decimal Importemo { get; set; }
        public int Idcuenta { get; set; }
        public int Idunidad { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenMoneda IdmonedaNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
    }
}
