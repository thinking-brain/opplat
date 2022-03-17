using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Inventario.Domain.Entities;

public class DetalleDeVale
{
    [Key]
    [Column(Order = 1)]
    public Guid ProductoId { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    [Key]
    [Column(Order = 2)]
    public Guid ValeId { get; set; }

    public virtual ValeDeSalida Vale { get; set; } = null!;

    public decimal Cantidad { get; set; }
}
