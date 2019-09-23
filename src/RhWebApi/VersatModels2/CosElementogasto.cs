using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RhWebApi.VersatModels2
{
    public class CosElementogasto
    {

        public int Idelementogasto { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool? Activo { get; set; }

      
        public virtual ICollection<CosSubelementogasto> CosSubelementogasto { get; set; }
     
    }
}
