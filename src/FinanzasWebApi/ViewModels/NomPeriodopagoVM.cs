using System;
using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public partial class NomPeriodopagoVM
    {
        
        public int Idperiodopago { get; set; }
        public int Idunidad { get; set; }
        public int? Anho { get; set; }
        public DateTime? PeriodoIni { get; set; }
        public DateTime? PeriodoFin { get; set; }
        public string StrIdentificador { get; set; }
        public int? Idprograma { get; set; }
        public decimal Coefsal { get; set; }

    }
}
