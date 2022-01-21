using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class ContratoId_FormaPagoId {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }
        public int FormaDePagoId { get; set; }
        public FormaDePago FormaDePago { get; set; }
    }
}