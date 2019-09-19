using System;
using System.Collections.Generic;

namespace ContabilidadWebApi.ViewModels
{
    public partial class NomDocumentoDetallePagoVM
    {
        public int Iddocumento { get; set; }
        public decimal? NCobrar { get; set; }
        public int? Idcuenta { get; set; }
        public int IdPeriodo { get; set; }

    }
}
