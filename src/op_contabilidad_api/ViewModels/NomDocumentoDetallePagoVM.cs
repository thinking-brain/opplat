using System;
using System.Collections.Generic;

namespace op_contabilidad_api.ViewModels
{
    public partial class NomDocumentoDetallePagoVM
    {
        public int Iddocumento { get; set; }
        public decimal? NCobrar { get; set; }
        public int? Idcuenta { get; set; }
        public int IdPeriodo { get; set; }

    }
}
