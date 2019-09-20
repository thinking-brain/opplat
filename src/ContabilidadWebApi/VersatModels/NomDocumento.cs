using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.VersatModels
{
    public partial class NomDocumento
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

        public virtual GenMoneda IdmonedaNavigation { get; set; }
        public virtual NomPeriodopago IdperiodopagoNavigation { get; set; }
        public virtual NomTipoDocumento IdtipodocumentoNavigation { get; set; }
        public virtual GenUnidadcontable IdunidadNavigation { get; set; }
        public virtual ICollection<NomAsiento> NomAsiento { get; set; }
        public virtual ICollection<NomDocumentoDetallePago> NomDocumentoDetallePago { get; set; }
    }
}
