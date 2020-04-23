using ContratacionWebApi.Models;

namespace ContratacionWebApi.Models {
    public class DetalleDeSolicitud {
        public int Id { get; set; }
        public int SolicitudDePagoId { get; set; }

        public virtual SolicitudDePago SolicitudDePago { get; set; }

        public int ConceptoId { get; set; }

        public virtual ConceptoDeSolicitud Concepto { get; set; }
        public decimal? Cantidad { get; set; }

        public decimal ImporteCup { get; set; }

        public decimal ImporteCuc { get; set; }
    }
}