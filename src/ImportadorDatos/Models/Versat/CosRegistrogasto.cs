using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class CosRegistrogasto
    {
        public CosRegistrogasto()
        {
            CosPasecentro = new HashSet<CosPasecentro>();
        }

        public int Idregistro { get; set; }
        public string Sumaclave { get; set; }
        public decimal? Importe { get; set; }

        public virtual ConRegistroanexo IdregistroNavigation { get; set; }
        public virtual ICollection<CosPasecentro> CosPasecentro { get; set; }
    }
}
