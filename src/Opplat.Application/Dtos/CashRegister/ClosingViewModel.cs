namespace Opplat.Application.Dtos.CashRegister;

public class ClosingViewModel
{
    public DateTime Date { get; set; }

    public decimal Cash { get; set; }

    public decimal PreviousCash { get; set; }

    public ClosingViewModel()
    {
    }
}
