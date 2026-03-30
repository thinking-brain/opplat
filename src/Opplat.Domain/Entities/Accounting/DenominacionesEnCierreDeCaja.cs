using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class DenominacionesEnCierreDeCaja
{
    [Key]
    [Column(Order = 1)]
    public Guid CierreDeCajaid { get; set; }

    public virtual CierreDeCaja? CierreDeCaja { get; set; }

    [Key]
    [Column(Order = 2)]
    public Guid DenominacionDeMonedaId { get; set; }

    public virtual DenominacionDeMoneda? DenominacionDeMoneda { get; set; }

    public int Cantidad { get; set; }
}
