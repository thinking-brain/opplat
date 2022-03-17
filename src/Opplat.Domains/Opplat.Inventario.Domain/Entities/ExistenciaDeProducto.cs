using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public class ExistenciaDeProducto: IEntity
{
    [Key]
    [Column(Order = 1)]
    public Guid ProductoId { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    [Key]
    [Column(Order = 2)]
    public Guid AlmacenId { get; set; }

    public virtual Almacen Almacen { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public DateTime ActualizadoEn { get; set; }
}
