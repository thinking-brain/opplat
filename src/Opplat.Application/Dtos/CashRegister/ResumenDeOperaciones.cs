
namespace Opplat.Application.Dtos.CashRegister;

public class ResumenDeOperaciones
{
    public DateTime Fecha { get; set; }

    public string Detalle { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public decimal Importe { get; set; }

    public string CentroDeCosto { get; set; } = string.Empty;

    public string Usuario { get; set; } = string.Empty;
}
