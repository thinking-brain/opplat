using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public class ClasificacionDeProducto
{
    public int Id { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<GrupoDeProducto> GruposDeProductos { get; set; }

    public ClasificacionDeProducto()
    {
        GruposDeProductos = new HashSet<GrupoDeProducto>();
        Descripcion = String.Empty;
    }

}

public class GrupoDeProducto
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = String.Empty;

    public int ClasificacionId { get; set; }

    public virtual ClasificacionDeProducto Clasificacion { get; set; } = null!;
}
public class Producto : Entity
{
    [Required]
    public string Nombre { get; set; } = String.Empty;

    public string Descripcion { get; set; } = String.Empty;

    public bool Activo { get; set; }

    public bool EsInventariable { get; set; }

    public int GrupoId { get; set; }

    public virtual GrupoDeProducto Grupo { get; set; } = null!;

    public override string ToString() => Nombre;

    public int UnidadDeMedidaId { get; set; }

    public virtual UnidadDeMedida UnidadDeMedida { get; set; } = null!;

    public decimal CantidadDeEnvase { get; set; }

    public decimal ProporcionDeMerma { get; set; }

    public decimal Costo { get; set; }

    public decimal ValorFijado { get; set; }
}
