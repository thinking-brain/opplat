using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class Disponibilidad
{
    [Key]
    public Guid CuentaId { get; set; }

    public virtual Cuenta? Cuenta { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Saldo { get; set; }
}
