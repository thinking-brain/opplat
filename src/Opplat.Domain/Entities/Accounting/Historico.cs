
namespace Opplat.Domain.Entities.Accounting;

public class Historico : BaseEntity
{
    public DateTime Fecha { get; set; }

    public required string Descripcion { get; set; }
}
