﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_costos_api.VersatModels2
{
    public partial class GenMoneda
    {

        [Key]
        public int Idmoneda { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public int Tipomoneda { get; set; }
        public bool? MantieneValorTasa { get; set; }
        public int Factor { get; set; }

      
        public virtual ICollection<ConCuentamoneda> ConCuentamoneda { get; set; }
       
    }
}
