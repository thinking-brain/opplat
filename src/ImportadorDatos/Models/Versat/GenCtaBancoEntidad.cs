using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat {
    public partial class GenCtaBancoEntidad {
        public int IdEntidad { get; set; }
        public string NumeroCta { get; set; }
        public int IdMoneda { get; set; }
        public int Idsucursal { get; set; }
    }
}