
using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

// TODO: Traslate to English
public static class MovementTypesConsts
{
    public const string Entrada = "Entrada";
    public const string SalidaAProduccion = "Salida a producción";
    public const string VentaIndependiente = "Venta independiente";
    public const string Merma = "Merma";
    public const string SalidaTrasladoInterno = "Salida traslado interno";
    public const string EntradaTrasladoInterno = "Entrada traslado interno";
    public const string EntradaPorErrorDeSalida = "Entrada por error en salida";
    public const string SalidaPorErrorDeEntrada = "Salida por error en entrada";
    public const string EntradaPorAjuste = "Entrada por ajuste";
    public const string SalidaPorAjuste = "Salida por ajuste";
    public const string SalidaConversionDeProducto = "Salida por conversion de producto";
    public const string EntradaConversionDeProducto = "Entrada por conversion de producto";
}

public class MovementType: Entity
{
    public string Description { get; set; } = String.Empty;

    public int Factor { get; set; }
}

public class ProductMovement: Entity
{
    public DateTime Date { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal Cost { get; set; }

    public int StorageId { get; set; }

    public virtual Storage Storage { get; set; } = null!;

    public int TypeId { get; set; }

    public MovementType Type { get; set; } = null!;

    public string Observations { get; set; } = String.Empty;
}
