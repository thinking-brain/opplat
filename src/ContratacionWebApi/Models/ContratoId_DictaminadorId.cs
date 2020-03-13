using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    [Table ("ContratoId_DictaminadorId")]
    public class ContratoId_DictaminadorId {
        public int Id { get; set; }

        [Required]
        public int ContratoId { get; set; }
        public virtual Contrato Contrato { get; set; }
        //trabajador
        public int JuridicoId { get; set; }
        public int EconomicoId { get; set; }
    }
}