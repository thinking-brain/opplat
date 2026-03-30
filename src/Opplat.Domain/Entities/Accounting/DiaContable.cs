using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

[Table("contb_dia_contable")]
public class DiaContable : BaseEntity
{
    public DateTime Fecha { get; set; }

    public bool Abierto { get; set; }

    public DateTime? HoraEnQueCerro { get; set; }

    public virtual ICollection<Asiento> Asientos { get; set; } = [];
}
