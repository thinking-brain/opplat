using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class Dictamen {
        public int Id { get; set; }
        [Required]
        public string Numero { get; set; }
        public string FilePath { get; set; }
        public string Observaciones { get; set; }
        public string FundamentosDeDerecho { get; set; }
        public string Consideraciones { get; set; }
        public string Recomendaciones { get; set; }
        public DateTime FechaDictamen { get; set; }
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }
        public string Username { get; set; }

        [Display (Name = "Otros SI")]
        public string OtrosSi { get; set; }

    }
}