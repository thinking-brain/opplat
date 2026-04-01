namespace Opplat.Domain.Entities.Accounting;

public class AccountLevel : BaseEntity
{
    public required string Number { get; set; }

    public required string Name { get; set; }

    public int? ParentLevelId { get; set; }

    public virtual AccountLevel? ParentLevel { get; set; }

    public virtual ICollection<AccountLevel> ChildLevels { get; set; } = [];
}
