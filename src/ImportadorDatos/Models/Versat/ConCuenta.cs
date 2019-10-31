using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImportadorDatos.Models.Versat
{
    public class ConCuenta
    {

        public int Idcuenta { get; set; }
        public string Clave { get; set; }
        public int Idapertura { get; set; }
        public bool? Activa { get; set; }

        public virtual ConApertura IdaperturaNavigation { get; set; }

        public virtual ICollection<OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }
        public virtual ICollection<OptCuentaCentroPeriodo> OptCuentaCentroPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodoOK> OptCuentaPeriodoOK { get; set; }
    }
}
