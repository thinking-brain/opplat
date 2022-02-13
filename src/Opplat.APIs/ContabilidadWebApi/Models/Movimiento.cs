using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_movimientos")]
    public class Movimiento
    {
        public int Id { get; set; }
        public int AsientoId { get; set; }

        public virtual Asiento Asiento { get; set; }

        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }

        public decimal Importe { get; set; }

        public TipoDeOperacion TipoDeOperacion { get; set; }
    }
}