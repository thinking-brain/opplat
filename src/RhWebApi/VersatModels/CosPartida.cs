﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RhWebApi.VersatModels
{
    public partial class CosPartida
    {

        public int Idpartida { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool? Activo { get; set; }

    }
}