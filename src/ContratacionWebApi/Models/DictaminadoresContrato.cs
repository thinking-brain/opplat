using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class DictaminadoresContrato {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
    }
}