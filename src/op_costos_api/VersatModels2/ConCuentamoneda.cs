using System;
using System.Collections.Generic;

namespace op_costos_api.VersatModels2
{
    public partial class ConCuentamoneda
    {

        public int Idcuenta { get; set; }
        public int Idmoneda { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual GenMoneda IdmonedaNavigation { get; set; }
    }
}
