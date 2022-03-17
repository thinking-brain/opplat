
namespace Opplat.Inventario.Domain.Entities;

public class DetalleEntrada
{
    public Guid ProductoId { get; set; }
    public Guid UnidadId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal ImporteTotal { get; set; }
}
