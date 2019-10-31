using System;
using System.Collections.Generic;
using System.Text;

namespace ContabilidadWebApi.Models
{
    public class CobroConCheque : Cobro
    {
        public int ChequeId { get; set; }

        public virtual Cheque Cheque { get; set; }
    }
}
