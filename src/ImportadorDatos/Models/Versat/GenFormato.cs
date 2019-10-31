using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenFormato
    {
        public GenFormato()
        {

        }

        public int Idformato { get; set; }
        public string Nombre { get; set; }
        public int Longitud { get; set; }
        public string Separador { get; set; }
        public bool? Enuso { get; set; }
    }
}
