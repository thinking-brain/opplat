using System;

namespace ContratacionWebApi.Models
{
    public class HistoricoEstadoContrato
    {
        public int Id { get; set; }

        public int ContratoId { get; set; }

        public virtual Contrato Contrato { get; set; }

        public EstadoOrden EstadoOrden { get; set; }

        public DateTime Fecha { get; set; }

        public string Usuario { get; set; }
    }
}