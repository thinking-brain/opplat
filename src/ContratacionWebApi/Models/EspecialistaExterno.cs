using System.ComponentModel.DataAnnotations;

namespace ContratacionWebApi.Models {
    public class EspecialistaExterno {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Cargo { get; set; }
    }
}