using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models
{
    public class HistoricoDeDocumento
    {
        public int Id { get; set; }

        public int DocumentoId { get; set; }

        public virtual Documento Documento { get; set; }

        [Required]
        public string Username { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public string Detalles { get; set; }
    }
}