using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

[Table("contb_centros_de_costo")]
public class CentroDeCosto : BaseEntity
{

    public required string Codigo { get; set; }

    public required string Nombre { get; set; }
}
