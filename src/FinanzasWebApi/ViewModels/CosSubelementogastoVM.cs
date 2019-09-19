using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanzasWebApi.ViewModels
{
    public partial class CosSubelementogastoVM
    {


        public int Idsubelemento { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Idelementogasto { get; set; }
        public int Idpartida { get; set; }
        public bool? Monnac { get; set; }
        public bool? Activo { get; set; }




    }
}
