
using Opplat.Shared.Entities;

namespace Opplat.Inventario.Domain.Entities;

public static class TipoDeMovimientoConstantes
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

public class TipoDeMovimiento: Entity
{
    public string Descripcion { get; set; } = String.Empty;

    public int Factor { get; set; }
}

public class MovimientoDeProducto: Entity
{
    public DateTime Fecha { get; set; }

    public int ProductoId { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal Costo { get; set; }

    public int AlmacenId { get; set; }

    public virtual Almacen Almacen { get; set; } = null!;

    public int TipoId { get; set; }

    public TipoDeMovimiento Tipo { get; set; } = null!;

    public string Observaciones { get; set; } = String.Empty;

    public string UsuarioId { get; set; } = String.Empty;
}
