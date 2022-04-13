
namespace Opplat.Contabilidad.Domain.Entities;
public class DenominacionDeMoneda
{
    public int Id { get; set; }

    public int MonedaId { get; set; }

    public virtual Moneda Moneda { get; set; }

    public decimal Valor { get; set; }

    public string Descripcion { get; set; }

    public bool Billete { get; set; }
}
