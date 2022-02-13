namespace ContabilidadWebApi.Models
{
    public class SubElementoDeGasto
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public string Codigo { get; set; }

        public bool MonedaNacional { get; set; }

        public bool Activo { get; set; }

        public int ElementoId { get; set; }

        public virtual ElementoDeGasto Elemento { get; set; }

        public int PartidaId { get; set; }

        public virtual PartidaDeGasto Partida { get; set; }
    }
}