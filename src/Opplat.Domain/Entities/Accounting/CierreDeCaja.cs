namespace Opplat.Domain.Entities.Accounting;

public class CierreDeCaja : BaseEntity
{

    public Guid DiaContableId { get; set; }

    public virtual DiaContable? DiaContable { get; set; }

    public DateTime Fecha { get; set; }

    public Guid CajaId { get; set; }

    public virtual Caja? Caja { get; set; }

    public virtual ICollection<DenominacionesEnCierreDeCaja> Desglose { get; set; } = [];
}
