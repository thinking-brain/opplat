using System;
using System.Collections.Generic;
using System.Text;

namespace ContabilidadWebApi.Models
{
    public class DocumentoContable
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Emisor { get; set; }

        public string Destinatario { get; set; }

        public decimal Importe { get; set; }

        public string Detalle { get; set; }
    }
}
