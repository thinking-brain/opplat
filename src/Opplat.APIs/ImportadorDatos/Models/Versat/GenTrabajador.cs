using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenTrabajador
    {
        public int Idtrabajador { get; set; }
        public string Codigo { get; set; }
        public string Numident { get; set; }
        public string Direccion { get; set; }
        public bool? Activo { get; set; }
        public string Nombres { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Nombre { get; set; }
    }
}
