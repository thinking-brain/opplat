using System;
using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public partial class NomDocumentoDetallePagoVM
    {
        public int Iddocumento { get; set; }
        public decimal? NCobrar { get; set; }
        public int? Idcuenta { get; set; }
        public int IdPeriodo { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }

    }
}
