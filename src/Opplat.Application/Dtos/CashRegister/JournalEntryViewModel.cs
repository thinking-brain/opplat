namespace Opplat.Application.Dtos.CashRegister;

public class JournalEntryViewModel
{
    public Guid CreditAccountId { get; set; }

    public Guid DebitAccountId { get; set; }

    public decimal Amount { get; set; }

    public string Observations { get; set; } = string.Empty;
}
