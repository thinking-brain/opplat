namespace ImportadorDatos.Models.EnlaceVersat {
    public class Entidad {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string NIT { get; set; }
        public int EntidadVersatId { get; set; }
        public int EntidadId { get; set; }
    }
}