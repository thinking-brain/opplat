using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Models
{
    [Table("cont_conceptos_de_solicitudes")]
    public class ConceptoDeSolicitud
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public Tipo Tipo { get; set; }

        public virtual ICollection<DetalleDeSolicitud> DetallesDeSolicitudes { get; set; }

        public ConceptoDeSolicitud()
        {
            DetallesDeSolicitudes = new HashSet<DetalleDeSolicitud>();
        }
    }
}