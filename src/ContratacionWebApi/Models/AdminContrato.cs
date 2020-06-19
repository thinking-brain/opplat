using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class AdminContrato {
        public int Id { get; set; }
        public int AdminContratoId { get; set; }
        public int DepartamentoId {get;set;}
        public Departamento Departamento {get;set;}
    }
}