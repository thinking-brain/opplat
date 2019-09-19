using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.VersatModels
{
    public partial class ConCuentanat
    {
      
        public int Idcuenta { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public short Naturaleza { get; set; }

    }
}
