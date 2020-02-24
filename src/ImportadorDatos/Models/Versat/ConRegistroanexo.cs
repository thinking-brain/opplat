using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class ConRegistroanexo
    {
        public int Idregistro { get; set; }
        public byte Idoperador { get; set; }
        public int Idpase { get; set; }
        public virtual ConPase IdpaseNavigation { get; set; }
        public virtual CosRegistrogasto CosRegistrogasto { get; set; }
    }
}
