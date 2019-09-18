using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_contabilidad_api.VersatModels2
{
    public  class CosCentro
    {

        [Key]
        public int Idcentro { get; set; }
        public string Clave { get; set; }
        public string Clavenivel { get; set; }
        public string Descripcion { get; set; }
        public int? Idapertura { get; set; }
        public bool? Activo { get; set; }
        public bool? Saldocero { get; set; }

        public virtual ICollection<OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }

    }
}
