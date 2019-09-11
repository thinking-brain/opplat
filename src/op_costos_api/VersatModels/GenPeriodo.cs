using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_costos_api.VersatModels
{
    public  class GenPeriodo
    {

        public int Idperiodo { get; set; }
        public byte Idejercicio { get; set; }
        public string Nombre { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public bool? Enuso { get; set; }

        public virtual ICollection<OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }
        public virtual ICollection<OptCuentaCentroPeriodo> OptCuentaCentroPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodoOK> OptCuentaPeriodoOK { get; set; }


    }
}
