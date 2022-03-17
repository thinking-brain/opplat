using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public class Aft : Entity
{
    public string Descripcion { get; set; }

    public DateTime FechaDeEntrada { get; set; }

    public decimal ValorInicial { get; set; }

    public decimal ValorActual { get; set; }

    public bool Baja { get; set; }

    public DateTime? FechaDeBaja { get; set; }
}