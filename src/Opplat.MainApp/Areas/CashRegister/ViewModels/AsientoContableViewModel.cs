namespace Opplat.MainApp.Areas.Caja.ViewModels;

public class AsientoContableViewModel
{
    public Guid CuentaCreditoId { get; set; }

    public Guid CuentaDebitoId { get; set; }

    public decimal Importe { get; set; }

    public string Observaciones { get; set; }
}
