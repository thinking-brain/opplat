using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    public class EspecialistaExterno {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public int EntidadId { get; set; }

        public Entidad Entidad { get; set; }

        public string Area { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }
        public List<EspExternoId_ContratoId> EspExternoId_ContratoId { get; set; }

        [NotMapped]
        private string nombreCompleto;

        [NotMapped]
        public string NombreCompleto {
            get { return Nombre + " " + Apellidos; }
            set { nombreCompleto = value; }
        }
    }
}