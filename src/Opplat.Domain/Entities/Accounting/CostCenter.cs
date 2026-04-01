namespace Opplat.Domain.Entities.Accounting;

public class CostCenter : BaseEntity
{

    public required string Code { get; set; }

    public required string Name { get; set; }
}
