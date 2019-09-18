using System;
using System.Collections.Generic;

namespace op_finanzas_api.ViewModels
{
    public partial class NomDocumentoVM
    {
      
 public int Iddocumento { get; set; }
        public string StrCodigo { get; set; }
        public string StrTitulo { get; set; }
        public DateTime? Fecha { get; set; }
        public int Idperiodopago { get; set; }
        public int Idestado { get; set; }
        public int Idtipodocumento { get; set; }
        public int Idunidad { get; set; }
        public int? Idactividad { get; set; }
        public int? Idmoneda { get; set; }
        public int? Idtasa { get; set; }

    }
}
