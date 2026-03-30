using Opplat.Domain.Dtos.Accounting;

namespace Opplat.Application.Dtos.CashRegister;

public class DesgloceEfectivoViewModel
{
    public ICollection<DenominacionViewModel> Billetes { get; set; } = [];

    public ICollection<DenominacionViewModel> Monedas { get; set; } = [];
}
