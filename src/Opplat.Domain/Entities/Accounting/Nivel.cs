using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class Nivel : BaseEntity
{
    [Required]
    [Display(Name = "Número")]
    public required string Numero { get; set; }

    [Required]
    public required string Nombre { get; set; }

    public int? NivelSuperiorId { get; set; }

    public virtual Nivel? NivelSuperior { get; set; }

    public virtual ICollection<Nivel> NivelesInferiores { get; set; } = [];
}
