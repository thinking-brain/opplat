using System;
using System.Collections.Generic;
using System.Text;

namespace ContabilidadWebApi.Models
{
    public class PagoConTransferencia : Pago
    {
        public int TransferenciaId { get; set; }

        public virtual Transferencia Transferencia { get; set; }
    }
}
