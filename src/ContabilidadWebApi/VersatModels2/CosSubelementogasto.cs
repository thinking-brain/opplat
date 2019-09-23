using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContabilidadWebApi.VersatModels2
{
    public partial class CosSubelementogasto
    {


        public int Idsubelemento { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Idelementogasto { get; set; }
        public int Idpartida { get; set; }
        public bool? Monnac { get; set; }
        public bool? Activo { get; set; }

        public virtual CosElementogasto IdelementogastoNavigation { get; set; }
        public virtual ICollection<OptCuentaCentroSubPeriodo> OptCuentaCentroSubPeriodo { get; set; }


    }
}
