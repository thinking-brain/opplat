using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Dtos.Accounting;

public record OperationDto
{
    public required string Type { get; set; }
    public OperationType OperationType { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public required string UserName { get; set; }
}
