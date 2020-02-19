namespace ContabilidadWebApi.Models
{
    public class PartidaDeGasto
    {
        public int Id { get; set; }

        public string Desripcion { get; set; }

        public string Codigo { get; set; }

        public bool Activo { get; set; }
    }
}