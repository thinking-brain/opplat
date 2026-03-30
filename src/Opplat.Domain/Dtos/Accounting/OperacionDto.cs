using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Dtos.Accounting;

public record OperacionDto
{
    public required string Tipo { get; set; }
    public TipoDeOperacion TipoDeOperacion{ get; set; }
    public required string Descripcion { get; set; }
    public decimal Importe { get; set; }
    public DateTime Fecha { get; set; }
    public required string Usuario { get; set; }
}
