using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat {
    public partial class GenSucursalBanco {
        public int IdSucursal { get; set; }
        public string Numero { get; set; }
        public string Direccion { get; set; }
        public int IdClasifBanco { get; set; }
    }
}