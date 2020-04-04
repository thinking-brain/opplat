using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class SolicitudDePago {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }
        public string Numero { get; set; }

        public int ContratoId { get; set; }

        public virtual Contrato Contrato { get; set; }

        public int ObjetoDeContratoId { get; set; }

        public virtual ObjetoDeContrato ObjetoDeContrato { get; set; }

        public EstadoSolicitud Estado { get; set; }

        public decimal ImporteTotalCup { get; set; }

        public decimal ImporteTotalCuc { get; set; }

        public string Observaciones { get; set; }

        public virtual ICollection<DetalleDeSolicitud> DetallesDeSolicitud { get; set; }

        public SolicitudDePago () {
            DetallesDeSolicitud = new HashSet<DetalleDeSolicitud> ();
        }
    }
}