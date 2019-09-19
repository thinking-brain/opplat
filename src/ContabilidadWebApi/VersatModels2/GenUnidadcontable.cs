﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContabilidadWebApi.VersatModels2
{
    public class GenUnidadcontable
    {
        [Key]
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
        public virtual ICollection<OptCuentaPeriodo> OptCuentaPeriodo { get; set; }
    }
}