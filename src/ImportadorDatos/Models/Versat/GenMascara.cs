using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenMascara
    {
        public GenMascara()
        {

        }

        public int Idmascara { get; set; }
        public int Idformato { get; set; }
        public string Nombre { get; set; }
        public byte Longitud { get; set; }
        public string Abreviatura { get; set; }
        public byte Posicion { get; set; }

        public virtual GenFormato IdformatoNavigation { get; set; }
    }
}
