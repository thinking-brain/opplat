using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;
public class Almacen: Entity
{
    public string Codigo { get; set; } = String.Empty;

    public string Descripcion { get; set; } = String.Empty;

    public bool IsCentroDeCosto { get; set; }
}
