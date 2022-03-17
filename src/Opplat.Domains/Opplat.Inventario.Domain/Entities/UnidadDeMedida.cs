using System.ComponentModel.DataAnnotations;
using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public class TipoDeUnidadDeMedida: Entity
{
    [Required]
    public string Name { get; set; }

    public virtual ICollection<UnidadDeMedida> UnidadesDeMedidas { get; set; }

    public TipoDeUnidadDeMedida()
    {
        UnidadesDeMedidas = new HashSet<UnidadDeMedida>();
        Name = String.Empty;
    }
}

public class UnidadDeMedida: Entity
{
    [Required]
    public string Nombre { get; set; } = String.Empty;

    [Required]
    public string Siglas { get; set; } = String.Empty;

    public Guid TipoDeUnidadDeMedidaId { get; set; }

    public virtual TipoDeUnidadDeMedida TipoDeUnidadDeMedida { get; set; } = null!;

    public decimal FactorDeConversion { get; set; }

    public override string ToString() => Nombre;
}
