﻿using System;
using System.Collections.Generic;

namespace op_costos_api.VersatModels2
{
    public partial class NomTipoDocumento
    {
       

        public int Idtipodocumento { get; set; }
        public string StrDescripcion { get; set; }

        public virtual ICollection<NomDocumento> NomDocumento { get; set; }
    }
}
