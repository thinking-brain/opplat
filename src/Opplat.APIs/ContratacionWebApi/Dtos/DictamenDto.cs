using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class DictamenDto {
        public int Id { get; set; }
        public string NumeroDeDictamen { get; set; }
        public string FilePath { get; set; }
        public string Observaciones { get; set; }
        public string FundamentosDeDerecho { get; set; }
        public string Consideraciones { get; set; }
        public string Recomendaciones { get; set; }
        public int ContratoId { get; set; }
        public string Username { get; set; }
        public string OtrosSi { get; set; }

    }
}