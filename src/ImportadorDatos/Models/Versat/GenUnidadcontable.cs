using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImportadorDatos.Models.Versat
{
    public class GenUnidadcontable
    {

        public int Idunidad { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool? Activo { get; set; }
        public string Direccion { get; set; }
        public int? Idnae { get; set; }
        public int? Idreup { get; set; }
        public int? Iddpa { get; set; }
        public string DirCorreo { get; set; }

        public virtual ICollection<OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }
        public virtual ICollection<OptCuentaCentroPeriodo> OptCuentaCentroPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
        public virtual ICollection<OptCuentaPeriodoOK> OptCuentaPeriodoOK { get; set; }
    }
}
