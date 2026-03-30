using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class DenominacionEnCaja
{
    [Key]
    [Column(Order = 1)]
    public Guid CajaId { get; set; }

    public virtual Caja? Caja { get; set; }

    [Key]
    [Column(Order = 2)]
    public Guid DenominacionId { get; set; }

    public virtual DenominacionDeMoneda? Denominacion { get; set; }

    public int Cantidad { get; set; }
}
