namespace Opplat.Domain.Entities.Accounting;

public class Caja : BaseEntity
{

    public required string Descripcion { get; set; }

    public virtual ICollection<DenominacionEnCaja> Efectivo { get; set; } = [];
}
