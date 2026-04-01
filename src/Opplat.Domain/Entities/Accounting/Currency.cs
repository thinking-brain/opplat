namespace Opplat.Domain.Entities.Accounting;

public class Currency : BaseEntity
{

    public required string Name { get; set; }

    public required string Symbol { get; set; }
}

