
namespace Opplat.Application.Dtos.CashRegister;

public class OperationsSummary
{
    public DateTime Date { get; set; }

    public string Detail { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CostCenter { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}
