using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models
{
    public class EspecialistaExterno {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        public int EntidadId { get; set; }

        [Required]
        public Entidad Entidad { get; set; }

        public string Area { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }

        [NotMapped]
        private string nombreCompleto;

        [NotMapped]
        public string NombreCompleto {
            get { return Nombre + " " + Apellidos; }
            set { nombreCompleto = value; }
        }
    }
}