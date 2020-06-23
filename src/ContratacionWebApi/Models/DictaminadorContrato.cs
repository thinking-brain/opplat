using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhWebApi.Models;

namespace ContratacionWebApi.Models {
    public class DictaminadorContrato {
        public int Id { get; set; }
        public int DictaminadorId { get; set; }
        public int DepartamentoId { get; set; }
        public Departamento Departamento { get; set; }
    }
}