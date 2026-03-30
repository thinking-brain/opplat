using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public enum TipoDeOperacion
{
    Credito,
    Debito,
}

public class Movimiento
{
    [Key]
    public Guid AsientoId { get; set; }

    public virtual Asiento? Asiento { get; set; }

    [Key]
    public Guid CuentaId { get; set; }

    public virtual Cuenta? Cuenta { get; set; }

    public decimal Importe { get; set; }

    public TipoDeOperacion TipoDeOperacion { get; set; }
}
