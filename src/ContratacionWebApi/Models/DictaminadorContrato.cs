using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class DictaminadorContrato {
        public int Id { get; set; }
        public int DictaminadorId { get; set; }
    }
}