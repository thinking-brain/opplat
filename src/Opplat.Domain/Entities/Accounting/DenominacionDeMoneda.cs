
namespace Opplat.Domain.Entities.Accounting;

public class DenominacionDeMoneda : BaseEntity
{
    public Guid MonedaId { get; set; }

    public virtual Moneda? Moneda { get; set; }

    public decimal Valor { get; set; }

    public required string Descripcion { get; set; }

    public bool Billete { get; set; }
}
