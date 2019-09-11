using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_costos_api.VersatModels2
{
    public  class OptCuentaCentroSubPeriodo
    {
        [Key]
        public int Idunidad { get; set; }
        [Key]
        public int Idperiodo { get; set; }
        [Key]
        public int Idcuenta { get; set; }
        [Key]
        public int Idcentro { get; set; }
        [Key]
        public int Idsub { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Clavecuenta { get; set; }
        public string Clavecentro { get; set; }
        public string Clavesub { get; set; }

        public virtual CosCentro IdcentroNavigation { get; set; }
        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual CosSubelementogasto IdsubNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }




    }
}
