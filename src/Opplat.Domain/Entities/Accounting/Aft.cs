namespace Opplat.Domain.Entities.Accounting;

public class Aft : BaseEntity
{
    public required string Descripcion { get; set; }

    public DateTime FechaDeEntrada { get; set; }

    public decimal ValorInicial { get; set; }

    public decimal ValorActual { get; set; }

    public bool Baja { get; set; }

    public DateTime? FechaDeBaja { get; set; }
}

