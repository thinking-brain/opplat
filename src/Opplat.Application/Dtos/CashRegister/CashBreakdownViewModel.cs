using Opplat.Domain.Dtos.Accounting;

namespace Opplat.Application.Dtos.CashRegister;

public class CashBreakdownViewModel
{
    public ICollection<DenominationViewModel> Banknotes { get; set; } = [];

    public ICollection<DenominationViewModel> Coins { get; set; } = [];
}
