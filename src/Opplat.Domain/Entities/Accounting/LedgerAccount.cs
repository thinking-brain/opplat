using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class LedgerAccount : BaseEntity
{
    public Guid LevelId { get; set; }

    public virtual AccountLevel? Level { get; set; }

    public AccountNature Nature { get; set; }

    public virtual AccountAvailability? Availability { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = [];

    [NotMapped]
    public string Number
    {
        get
        {
            if (Level == null)
            {
                return "+++" + Id;
            }
            var number = Level.Number;
            var level = Level.ParentLevel;
            while (level != null)
            {
                number = level.Number + "-" + number;
                level = level.ParentLevel;
            }
            return number;
        }
    }

    [NotMapped]
    public string Name
    {
        get { return Level != null ? Level.Name : "Account " + Id; }
    }

    [NotMapped]
    public bool IsValid
    {
        get { return Level != null && Level.ChildLevels.Count == 0; }
    }

}

