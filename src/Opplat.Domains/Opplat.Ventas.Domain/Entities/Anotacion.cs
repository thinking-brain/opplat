
using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public class Anotacion: Entity
{
    public DateTime Fecha { get; set; }

    public Guid OrigenId { get; set; }

    public virtual Almacen Origen { get; set; }

    public Guid? DestinoId { get; set; }

    public virtual Almacen? Destino { get; set; }

    public string Descripcion { get; set; }

    public string ConfeccionadoPor { get; set; }
    public string AutorizadoPor { get; set; }

    public virtual ICollection<DetalleDeVale> Productos { get; set; }

    public ValeDeSalida()
    {
        Fecha = DateTime.UtcNow;
        Origen = null!;
        Descripcion = String.Empty;
        ConfeccionadoPor = String.Empty;
        AutorizadoPor = String.Empty;
        Productos = new HashSet<DetalleDeVale>();
    }
}
