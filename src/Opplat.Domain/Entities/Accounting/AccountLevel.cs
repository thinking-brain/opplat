using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class AccountLevel : BaseEntity
{
    [Required]
    [Display(Name = "Number")]
    public required string Number { get; set; }

    [Required]
    public required string Name { get; set; }

    public int? ParentLevelId { get; set; }

    public virtual AccountLevel? ParentLevel { get; set; }

    public virtual ICollection<AccountLevel> ChildLevels { get; set; } = [];
}
