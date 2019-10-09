using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenUsuario
    {
        public GenUsuario()
        {

        }

        public int Idusuario { get; set; }
        public string Loginusuario { get; set; }
        public string Nombre { get; set; }
        public byte Tipo { get; set; }
        public bool? Activo { get; set; }
    }
}
