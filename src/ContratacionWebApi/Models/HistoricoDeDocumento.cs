using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models
{
    public class HistoricoDeDocumento
    {
        public int Id { get; set; }

        public int DocumentoDeContratoId { get; set; }

        public virtual DocumentoDeContrato DocumentoDeContrato { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        public string Detalles { get; set; }
    }
}