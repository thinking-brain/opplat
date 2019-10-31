using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImportadorDatos.Models.Versat
{
    public partial class GenMoneda
    {


        public int Idmoneda { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public int Tipomoneda { get; set; }
        public bool? MantieneValorTasa { get; set; }
        public int Factor { get; set; }


        public virtual ICollection<ConCuentamoneda> ConCuentamoneda { get; set; }

    }
}
