using System;
using System.Collections.Generic;

namespace RhWebApi.VersatModels2
{
    public partial class FinDocumento
    {

        public int Iddocumento { get; set; }
        public string Nrodoc { get; set; }
        public byte Estado { get; set; }
        public string Descripcion { get; set; }
        public int Idregdoc { get; set; }
        public bool? Validado { get; set; }
        public int Idactividad { get; set; }
        public int Idunidad { get; set; }
        public DateTime Fechaemision { get; set; }
        public DateTime Fechaprocesam { get; set; }

        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
    }
}
