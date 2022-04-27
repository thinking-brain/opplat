using System.ComponentModel.DataAnnotations;

namespace Opplat.Contabilidad.Domain.Entities;

public enum TipoDeOperacion
{
    Credito,
    Debito,
}

public class Movimiento
{
    [Key]
    public int AsientoId { get; set; }

    public virtual Asiento Asiento { get; set; }

    [Key]
    public int CuentaId { get; set; }

    public virtual Cuenta Cuenta { get; set; }

    public decimal Importe { get; set; }

    public TipoDeOperacion TipoDeOperacion { get; set; }
}
