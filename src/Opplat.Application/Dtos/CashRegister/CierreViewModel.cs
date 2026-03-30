namespace Opplat.Application.Dtos.CashRegister;

public class CierreViewModel
{
    public DateTime Fecha { get; set; }

    //public List<ICierreContableElement> Detalles { get; set; }

    public decimal Efectivo { get; set; }

    public decimal EfectivoAnterior { get; set; }

    //public List<DenominacionesEnCierreDeCaja> Desgloce { get; set; }

    public CierreViewModel()
    {
        // Desgloce = new List<DenominacionesEnCierreDeCaja>();
        // Detalles = new List<ICierreContableElement>();
    }
}
