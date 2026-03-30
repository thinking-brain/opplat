using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class Currency : BaseEntity
{

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Symbol { get; set; }
}

