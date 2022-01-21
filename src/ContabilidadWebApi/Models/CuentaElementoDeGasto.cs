namespace ContabilidadWebApi.Models
{
    public class CuentaElementoDeGasto
    {
        public int Id { get; set; }

        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }

        public int ElementoId { get; set; }

        public virtual ElementoDeGasto Elemento { get; set; }
    }
}