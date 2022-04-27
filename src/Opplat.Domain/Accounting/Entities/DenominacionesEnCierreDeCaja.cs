using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Contabilidad.Domain.Entities;
public class DenominacionesEnCierreDeCaja
{
    [Key]
    [Column(Order = 1)]
    public int CierreDeCajaid { get; set; }

    public virtual CierreDeCaja CierreDeCaja { get; set; }

    [Key]
    [Column(Order = 2)]
    public int DenominacionDeMonedaId { get; set; }

    public virtual DenominacionDeMoneda DenominacionDeMoneda { get; set; }

    public int Cantidad { get; set; }
}
