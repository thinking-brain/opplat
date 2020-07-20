using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class Monto {
        public int Id { get; set; }

        [DataType (DataType.Currency)]
        public decimal Cantidad { get; set; }
        public Moneda Moneda { get; set; }
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }
    }
}