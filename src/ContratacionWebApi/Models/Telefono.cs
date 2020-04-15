namespace ContratacionWebApi.Models {
    public class Telefono {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Extension { get; set; }
        public int? EntidadId { get; set; }
        public Entidad Entidad { get; set; }
    }
}