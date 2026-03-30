using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class Asiento : BaseEntity
{
    public Guid DiaContableId { get; set; }

    public virtual DiaContable? DiaContable { get; set; }

    public DateTime Fecha { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = [];

    public required string Usuario { get; set; }

    public required string Detalle { get; set; }

    [NotMapped]
    public bool EsValido
    {
        get
        {
            return Movimientos.Where(m => m.TipoDeOperacion == TipoDeOperacion.Credito).Sum(c => c.Importe) == Movimientos.Where(m => m.TipoDeOperacion == TipoDeOperacion.Debito).Sum(d => d.Importe);
        }
    }
}
