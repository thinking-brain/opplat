using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class Dictamen {
        public int Id { get; set; }
        public string NumeroDeDictamen { get; set; }
        public int EspecialistaId { get; set; }

        public virtual Especialista Especialista { get; set; }

        public bool Aprobado { get; set; }

        public string Observaciones { get; set; }

        [Required]
        public string FundamentosDeDerecho { get; set; }

        public string Consideraciones { get; set; }

        public string Recomendaciones { get; set; }

        [Display (Name = "Otros SI")]
        public string OtrosSi { get; set; }

    }
}