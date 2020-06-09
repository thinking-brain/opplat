using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat {
    public partial class GenSucursalBanco {
        public int Idsucursal { get; set; }
        public string Numero { get; set; }
        public string Direccion { get; set; }
        public int Idclasifbanco { get; set; }
    }
}