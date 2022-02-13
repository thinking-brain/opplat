using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public class GenArea
    {
        public GenArea()
        {

        }

        public int Idarea { get; set; }
        public string Clave { get; set; }
        public string Clavenivel { get; set; }
        public string Descripcion { get; set; }
        public int Idapertura { get; set; }
        public int Idunidad { get; set; }
        public bool? Activa { get; set; }
        public virtual GenAperturaarea IdaperturaNavigation { get; set; }
    }
}
