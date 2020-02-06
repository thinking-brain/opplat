namespace ContabilidadWebApi.Models
{
    public class RegistroDeGasto
    {
        public int Id { get; set; }

        public int AsientoId { get; set; }

        public virtual Asiento Asiento { get; set; }

        public int SubElementoId { get; set; }

        public virtual SubElementoDeGasto SubElemento { get; set; }
    }
}