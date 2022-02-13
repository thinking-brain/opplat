﻿using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class OptCuentaPeriodo
    {
        public int Idunidad { get; set; }
        public int Idperiodo { get; set; }
        public int Idcuenta { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public string Clave { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
    }
}
