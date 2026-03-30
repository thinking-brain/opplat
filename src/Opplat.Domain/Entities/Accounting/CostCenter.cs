using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

[Table("contb_centros_de_costo")]
public class CostCenter : BaseEntity
{

    public required string Code { get; set; }

    public required string Name { get; set; }
}
