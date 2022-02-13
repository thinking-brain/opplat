﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace op_costos_api.VersatModels2
{
    public partial class CosPartida
    {
        [Key]
        public int Idpartida { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool? Activo { get; set; }

    }
}
