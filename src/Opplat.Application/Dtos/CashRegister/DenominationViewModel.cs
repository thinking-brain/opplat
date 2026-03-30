namespace Opplat.Application.Dtos.CashRegister;
public class DenominationViewModel
{
    public decimal Value { get; set; }

    public bool Cup { get; set; }

    public bool Cuc { get; set; }

    public int CupQuantity { get; set; }

    public int CucQuantity { get; set; }

    public int CupExtractionQuantity { get; set; }

    public int CucExtractionQuantity { get; set; }
}
