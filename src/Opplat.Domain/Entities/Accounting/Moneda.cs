using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class Moneda : BaseEntity
{

    [Required]
    public required string Nombre { get; set; }

    [Required]
    public required string Sigla { get; set; }
}

