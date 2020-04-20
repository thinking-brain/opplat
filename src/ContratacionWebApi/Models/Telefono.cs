using System.ComponentModel.DataAnnotations;

namespace ContratacionWebApi.Models {
    public class Telefono {
        public int Id { get; set; }

        [DataType (DataType.PhoneNumber)]
        public string Numero { get; set; }

        [DataType (DataType.PhoneNumber)]
        public string Extension { get; set; }
        public int? EntidadId { get; set; }
        public Entidad Entidad { get; set; }
    }
}