using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Entities;
using Opplat.Inventario.Domain.Repositories;

namespace Opplat.Inventario.Domain.Services;

public class MovimientoResult
{
    public bool Puede { get; set; }

    public decimal Cantidad { get; set; }

    public decimal CantidadConvertida { get; set; }

    public string Unidad { get; set; } = String.Empty;
}

public interface IInventarioService
{
    
}

public class InventarioService
{
    private IAlmacenRepository _almacenRepo;
    private IExistenciaRepository _existenciaRepo;
    private IProductoRepository _prodRepo;
    private IUnidadMedidaRepository _unidadRepo;
    private ITipoMovimientoRepository _tipoMovRepo;

    public InventarioService(IAlmacenRepository almacenRepo, IExistenciaRepository existenciaRepo,
        IProductoRepository prodRepo, IUnidadMedidaRepository unidadRepo, ITipoMovimientoRepository tipoMovRepo)
    {
        _almacenRepo = almacenRepo;
        _existenciaRepo = existenciaRepo;
        _prodRepo = prodRepo;
        _unidadRepo = unidadRepo;
        _tipoMovRepo = tipoMovRepo;
    }

    /// <summary>
    /// Crea movimiento de producto en un Almacen especificado por la cantidad definida
    /// </summary>
    /// <param name="producto">Producto Concreto</param>
    /// <param name="almacen"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedida"></param>
    /// <param name="fecha"></param>
    /// <param name="tipoDeMovimiento"></param>
    /// <param name="usuarioId"></param>
    /// <param name="observaciones"></param>
    /// <returns></returns>
    private async Task<MovimientoResult> CrearMovimiento(Producto producto, Almacen almacen, decimal cantidad, UnidadDeMedida unidadDeMedida, DateTime fecha
        , TipoDeMovimiento tipoDeMovimiento, string usuarioId, string observaciones = "")
    {
        //todo: tener en cuenta la unidad u, si es esa unidad se mueve por la cantidad que tiene definida en envase
        decimal cantidadAjustada = cantidad * (producto.UnidadDeMedida.FactorDeConversion / unidadDeMedida.FactorDeConversion);
        var movimiento = new MovimientoDeProducto()
        {
            Almacen = almacen,
            Producto = producto,
            Cantidad = cantidadAjustada,
            Fecha = fecha,
            Tipo = tipoDeMovimiento,
            Costo = cantidadAjustada * producto.Costo,
            UsuarioId = usuarioId,
            Observaciones = observaciones
        };
        var existencia = await _existenciaRepo.Get(almacen.Id, producto.Id);
        if (existencia == null)
        {
            existencia = new ExistenciaDeProducto()
            {
                Almacen = almacen,
                Producto = producto,
                Cantidad = 0,
                ActualizadoEn = DateTime.UtcNow,
            };
            var response = await _existenciaRepo.Create(existencia);
            // TODO: Analizar si implementar accion a realizar si no se crea una nueva existencia
        }
        if (existencia.Cantidad + (cantidadAjustada * tipoDeMovimiento.Factor) < 0)
        {
            var cantidadFaltante = Math.Abs(existencia.Cantidad + (cantidadAjustada * tipoDeMovimiento.Factor));
            return new MovimientoResult
            {
                Puede = false,
                Cantidad = cantidadFaltante,
                CantidadConvertida = cantidadAjustada,
                Unidad = producto.UnidadDeMedida.Siglas
            };
        }
        existencia.Cantidad = existencia.Cantidad + (cantidadAjustada * tipoDeMovimiento.Factor);
        await _almacenRepo.AddMovimiento(movimiento);
        return new MovimientoResult { Puede = true };
    }

    /// <summary>
    /// Traslado de productos entre Almacenes
    /// </summary>
    /// <param name="origenId"></param>
    /// <param name="destinoId"></param>
    /// <param name="productoId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedidaId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<bool> TrasladarProductoDeAlmacen(string origenId, string destinoId, string productoId, decimal cantidad, string unidadDeMedidaId, string usuarioId)
    {
        var date = DateTime.UtcNow;
        var producto = await _prodRepo.Find(productoId);
        var origen = await _almacenRepo.Find(origenId);
        var destino = await _almacenRepo.Find(destinoId);
        var unidad = await _unidadRepo.Find(unidadDeMedidaId);
        var movimientoSalida = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.SalidaTrasladoInterno);
        var movimientoEntrada = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.EntradaTrasladoInterno);
        var result = await CrearMovimiento(producto, origen, cantidad, unidad, date, movimientoSalida, usuarioId);
        if (!result.Puede) return false;
        result = await CrearMovimiento(producto, destino, cantidad, unidad, date, movimientoEntrada, usuarioId);
        return result.Puede;
    }

    public async Task<MovimientoResult> DarEntrada(string productoId, string almacenId, decimal cantidad, string unidadDeMedidaId, string usuarioId)
    {
        var tipoDeMovEntrada = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.Entrada);
        return await DarEntrada(productoId, almacenId, tipoDeMovEntrada.Id.ToString(), cantidad, unidadDeMedidaId, usuarioId);
    }

    public async Task<MovimientoResult> DarEntrada(string productoId, string almacenId, string tipoDeMovimientoId, decimal cantidad, string unidadDeMedidaId, string usuarioId)
    {
        var producto = await _prodRepo.Find(productoId);
        var centroDeCosto = await _almacenRepo.Find(almacenId);
        var tipoDeMovimiento = await _tipoMovRepo.Find(tipoDeMovimientoId);
        var unidad = await _unidadRepo.Find(unidadDeMedidaId);
        return await CrearMovimiento(producto, centroDeCosto, cantidad, unidad, DateTime.UtcNow, tipoDeMovimiento, usuarioId);
    }

    /// <summary>
    /// Dar salida por venta
    /// </summary>
    /// <param name="productoId"></param>
    /// <param name="almacenId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedidaId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<MovimientoResult> DarSalida(string productoId, string almacenId, decimal cantidad, string unidadDeMedidaId, string usuarioId, string observaciones = "")
    {
        var tipoDeMovEntrada = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.SalidaAProduccion);
        return await DarSalida(productoId, almacenId, tipoDeMovEntrada.Id.ToString(), cantidad, unidadDeMedidaId, usuarioId, observaciones);
    }

    /// <summary>
    /// Dar salida especificando tipo de movimiento
    /// </summary>
    /// <param name="productoId"></param>
    /// <param name="almacenId"></param>
    /// <param name="tipoDeMovimientoId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedidaId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<MovimientoResult> DarSalida(string productoId, string almacenId, string tipoDeMovimientoId, decimal cantidad, string unidadDeMedidaId, string usuarioId, string observaciones = "")
    {
        var producto = await _prodRepo.Find(productoId);
        var almacen = await _almacenRepo.Find(almacenId);
        var tipoDeMovimiento = await _tipoMovRepo.Find(tipoDeMovimientoId);
        var unidad = await _unidadRepo.Find(unidadDeMedidaId);
        var date = DateTime.UtcNow;
        return await CrearMovimiento(producto, almacen, cantidad, unidad, date, tipoDeMovimiento, usuarioId, observaciones);
    }

    /// <summary>
    /// Dar salida por merma
    /// </summary>
    /// <param name="productoId"></param>
    /// <param name="almacenId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedidaId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<MovimientoResult> DarSalidaPorMerma(string productoId, string almacenId, decimal cantidad, string unidadDeMedidaId, string usuarioId, string observaciones)
    {
        var tipoDeMovimiento = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.Merma);
        var isValid = await DarSalida(productoId, almacenId, tipoDeMovimiento.Id.ToString(), cantidad, unidadDeMedidaId, usuarioId, observaciones);
        return isValid;
    }

    /// <summary>
    /// Convierte de un producto a otro una cantidad especificada
    /// </summary>
    /// <param name="almacenId"></param>
    /// <param name="productoOrigenId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadId"></param>
    /// <param name="productoDestinoId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<MovimientoResult> ConvertirDeUnProductoAOtro(string almacenId, string productoOrigenId, decimal cantidad, string unidadId, string productoDestinoId, string usuarioId)
    {
        var tipoDeMovimientoS = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.SalidaConversionDeProducto);
        var tipoDeMovimientoE = await _tipoMovRepo.FindByDescription(TipoDeMovimientoConstantes.EntradaConversionDeProducto);
        var result = await DarSalida(productoOrigenId, almacenId, tipoDeMovimientoS.Id.ToString(), cantidad, unidadId, usuarioId);
        if (result.Puede)
        {
            result = await DarEntrada(productoDestinoId, almacenId, tipoDeMovimientoE.Id.ToString(), cantidad, unidadId, usuarioId);
        }
        return result;
    }

    public async Task<Guid> GetAlmacenIdPorDefecto()
    {
        var almacen = await _almacenRepo.GetPrimaryStorage();
        if (almacen != null)
        {
            // TODO: Valorar lanzar Excepciones personalizadas
            throw new Exception("No existe ningun almacen.");
        }
        return almacen!.Id;
    }

    public async Task<IEnumerable<ValeDeSalida>> ValesDeSalida(DateTime? fechaInicio = null, DateTime? fechaFinal = null)
    {
        fechaInicio = fechaInicio ?? DateTime.MinValue;
        fechaFinal = fechaFinal ?? DateTime.UtcNow;
        var vales = await _almacenRepo.GetValesDeSalida(fechaInicio.Value, fechaFinal.Value);
        return vales;
    }

    public async Task<IEnumerable<DetalleDeVale>> DetalleDeVale(Guid valeId)
    {
        var detalles = await _almacenRepo.GetDetallesDeVale(valeId);
        return detalles;
    }

    public async Task<IEnumerable<MovimientoDeProducto>> Mermas()
    {
        // TODO: Adaptar MovDeProd a Merma que se usaba antes en la vista de almacen.
        var almacen = await _almacenRepo.GetPrimaryStorage();
        var mermas = await _almacenRepo.GetMermas(almacen.Id);
        return mermas;
    }

    /// <summary>
    /// Dar entrada a producto
    /// </summary>
    /// <param name="productoId"></param>
    /// <param name="unidadId"></param>
    /// <param name="cantidad"></param>
    /// <param name="importe"></param>
    /// <param name="usuarioId"></param>
    public async Task DarEntradaAProducto(Guid productoId, Guid unidadId, decimal cantidad, decimal importe, string usuarioId)
    {
        var producto = await _prodRepo.Find(productoId);
        var unidad = await _unidadRepo.Find(unidadId);
        await EntrarProducto(producto, unidad, cantidad, importe, usuarioId);
    }

    public async Task<bool> DarEntradaAProductoYTransferirACentroDeCosto(IEnumerable<DetalleEntrada> productos, string usuarioId, string centroDeCostoId)
    {
        var result = true;
        var centroDeCosto = await _almacenRepo.Find(centroDeCostoId);
        var detalles = new List<DetalleDeVale>();
        foreach (var detail in productos)
        {
            await DarEntradaAProducto(detail.ProductoId, detail.UnidadId, detail.Cantidad, detail.ImporteTotal, usuarioId);
            var producto = await _prodRepo.Find(detail.ProductoId.ToString());
            var unidad = await _unidadRepo.Find(detail.UnidadId.ToString());
            var cantidadConvertida = detail.Cantidad * (producto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
            detalles.Add(new DetalleDeVale()
            {
                Cantidad = cantidadConvertida,
                ProductoId = detail.ProductoId,
            });
        }
        var vale = new ValeDeSalida()
        {
            OrigenId = await GetAlmacenIdPorDefecto(),
            DestinoId = new Guid(centroDeCostoId),
            Descripcion = "Salida para centro de costo " + centroDeCosto.Descripcion,
            Productos = detalles,
            ConfeccionadoPor = usuarioId,
            AutorizadoPor = usuarioId,
        };
        result = await DarSalidaDeAlmacen(vale);
        return result;
    }


    /// <summary>
    /// Entrada desde centro de costo
    /// </summary>
    /// <param name="compra"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<bool> EntradaDesdeCentroDeCosto(Compra compra, int centroDeCostoId, string usuarioId)
    {
        if (!compra.Productos.Any())
        {
            return false;
        }
        var almacenId = await this.GetAlmacenIdPorDefecto();
        foreach (var prod in compra.Productos)
        {
            var result = await this.EntradaDesdeCentroDeCosto(almacenId, centroDeCostoId, prod.ProductoId, prod.Cantidad, prod.UnidadDeMedidaId, usuarioId);
            if (!result)
            {
                return false;
            }
        }
        await _db.SaveChangesAsync();
        return true;
    }


    /// <summary>
    /// Entrada desde centro de costo
    /// </summary>
    /// <param name="almacenId"></param>
    /// <param name="centroCostoId"></param>
    /// <param name="productoId"></param>
    /// <param name="cantidad"></param>
    /// <param name="unidadDeMedidaId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<bool> EntradaDesdeCentroDeCosto(int almacenId, int centroCostoId, int productoId, decimal cantidad,
        int unidadDeMedidaId, string usuarioId)
    {
        var tipoSalida = await _db.Set<TipoDeMovimiento>()
                        .SingleOrDefaultAsync(t => t.Descripcion == TipoDeMovimientoConstantes.SalidaPorErrorDeEntrada);
        var tipoEntrada = await _db.Set<TipoDeMovimiento>()
                        .SingleOrDefaultAsync(t => t.Descripcion == TipoDeMovimientoConstantes.EntradaPorErrorDeSalida);

        var producto = await _db.Set<ProductoConcreto>().SingleOrDefaultAsync(p => p.ProductoId == productoId);
        var unidad = await _db.Set<UnidadDeMedida>().FindAsync(unidadDeMedidaId);
        MovimientoResult result = null;
        if (producto.ProporcionDeMerma > 0)
        {
            //var cantidadConvertida = cantidad * (producto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
            result = await _centroDeCostoService.DarEntrada(productoId, centroCostoId, tipoEntrada.Id, cantidad * producto.ProporcionDeMerma, unidad.Id, usuarioId);
        }
        if (!result.Puede)
        {
            return false;
        }
        result = await _centroDeCostoService.DarSalida(productoId, centroCostoId, tipoSalida.Id, cantidad, unidadDeMedidaId, usuarioId);
        if (result.Puede)
        {
            await EntradaDesdeCentroDeCosto(productoId, unidadDeMedidaId, cantidad, usuarioId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Entrada de producto a almacen desde centro de costo
    /// </summary>
    /// <param name="productoId"></param>
    /// <param name="unidadId"></param>
    /// <param name="cantidad"></param>
    /// <param name="usuarioId"></param>
    public async Task EntradaDesdeCentroDeCosto(int productoId, int unidadId, decimal cantidad, string usuarioId)
    {
        var producto = await _db.Set<ProductoConcreto>().SingleOrDefaultAsync(p => p.ProductoId == productoId);
        var unidad = await _db.Set<UnidadDeMedida>().FindAsync(unidadId);
        await EntradaDesdeCentroDeCosto(producto, unidad, cantidad, usuarioId);
    }

    /// <summary>
    /// Entrada a almacen desde centro de costo
    /// </summary>
    /// <param name="producto"></param>
    /// <param name="unidad"></param>
    /// <param name="cantidad"></param>
    /// <param name="usuarioId"></param>
    private async Task EntradaDesdeCentroDeCosto(ProductoConcreto producto, UnidadDeMedida unidad, decimal cantidad, string usuarioId)
    {
        var diaContable = await _periodoContableService.GetDiaContableActual();
        var cantidadConvertida = cantidad * (producto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
        var entradaAlmacen = new EntradaAlmacen() { ProductoId = producto.Id, AlmacenId = await GetAlmacenIdPorDefecto(), Cantidad = cantidadConvertida, UsuarioId = usuarioId, DiaContableId = diaContable.Id, Fecha = DateTime.Now };
        await EntrarProducto(entradaAlmacen, usuarioId);
    }

    /// <summary>
    /// Efectuar entrada de producto a almacen calculando con importe
    /// </summary>
    /// <param name="entrada"></param>
    /// <param name="importe"></param>
    /// <param name="usuarioId"></param>
    private async Task EntrarProducto(EntradaAlmacen entrada, decimal importe, string usuarioId)
    {
        decimal existenciaTotal = 0m;
        if (await _db.Set<ExistenciaCentroDeCosto>().AnyAsync(e => e.ProductoId == entrada.ProductoId))
        {
            existenciaTotal += await _db.Set<ExistenciaCentroDeCosto>().Where(e => e.ProductoId == entrada.ProductoId).SumAsync(e => e.Cantidad);
        }
        if (await _db.Set<ExistenciaAlmacen>().AnyAsync(e => e.ProductoId == entrada.ProductoId))
        {
            existenciaTotal += await _db.Set<ExistenciaAlmacen>().Where(e => e.ProductoId == entrada.ProductoId).SumAsync(e => e.ExistenciaEnAlmacen);
        }
        var producto = await _db.Set<ProductoConcreto>().FindAsync(entrada.ProductoId);
        var precioUnitarioNuevo = (existenciaTotal * producto.PrecioUnitario +
                                   importe) / (existenciaTotal +
                                  entrada.Cantidad);
        producto.PrecioUnitario = precioUnitarioNuevo;
        _db.Entry(producto).State = EntityState.Modified;

        await EntrarProducto(entrada, usuarioId);

        var elaboracionesAfectadas =
            await _db.Set<Elaboracion>()
                .Include(e => e.Productos)
                .Where(e => e.Productos.Any(p => p.ProductoId == entrada.Producto.ProductoId))
                .ToListAsync();
        foreach (var el in elaboracionesAfectadas)
        {
            await _elaboracionService.CalcularCosto(el);
        }
    }

    /// <summary>
    /// Efectuar entrada de producto
    /// </summary>
    /// <param name="entrada"></param>
    /// <param name="usuarioId"></param>
    private async Task EntrarProducto(EntradaAlmacen entrada, string usuarioId)
    {
        if (String.IsNullOrEmpty(usuarioId) && String.IsNullOrEmpty(entrada.UsuarioId))
        {
            return;
        }
        if (!String.IsNullOrEmpty(usuarioId))
        {
            entrada.UsuarioId = usuarioId;
        }
        var contbService = new PeriodoContableService(_db);
        var diaContable = await contbService.GetDiaContableActual();
        entrada.DiaContableId = diaContable.Id;
        entrada.Fecha = DateTime.Now;
        _db.Set<EntradaAlmacen>().Add(entrada);

        ExistenciaAlmacen existencia;
        if (await _db.Set<ExistenciaAlmacen>().AnyAsync(e => e.ProductoId.Equals(entrada.ProductoId)
                                                                && e.AlmacenId.Equals(entrada.AlmacenId)))
        {
            existencia =
                await _db.Set<ExistenciaAlmacen>().SingleOrDefaultAsync(e => e.ProductoId.Equals(entrada.ProductoId)
                                                                && e.AlmacenId.Equals(entrada.AlmacenId));
            existencia.ExistenciaEnAlmacen += entrada.Cantidad;
            _db.Entry(existencia).State = EntityState.Modified;
        }
        else
        {
            existencia = new ExistenciaAlmacen() { AlmacenId = entrada.AlmacenId, ProductoId = entrada.ProductoId, ExistenciaEnAlmacen = entrada.Cantidad };
            _db.Set<ExistenciaAlmacen>().Add(existencia);
        }
    }

    /// <summary>
    /// Entrar producto a almacen
    /// </summary>
    /// <param name="producto"></param>
    /// <param name="unidad"></param>
    /// <param name="cantidad"></param>
    /// <param name="importe"></param>
    /// <param name="usuarioId"></param>
    private async Task EntrarProducto(Producto producto, UnidadDeMedida unidad, decimal cantidad, decimal importe, string usuarioId)
    {
        var prodConcreto = await _productoService.GetProductoConcreto(producto, unidad, cantidad, importe);
        var diaContable = await _periodoContableService.GetDiaContableActual();
        var cantidadConvertida = cantidad * (prodConcreto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
        var entradaAlmacen = new EntradaAlmacen() { ProductoId = prodConcreto.Id, AlmacenId = await GetAlmacenIdPorDefecto(), Cantidad = cantidadConvertida, UsuarioId = usuarioId, DiaContableId = diaContable.Id, Fecha = DateTime.Now };
        await EntrarProducto(entradaAlmacen, importe, usuarioId);
    }

    /// <summary>
    /// Dar salida de almacen (salva en BD)
    /// </summary>
    /// <param name="vale"></param>
    /// <returns></returns>
    public async Task<bool> DarSalidaDeAlmacen(ValeSalidaDeAlmacen vale)
    {
        vale.Fecha = DateTime.Now;
        _db.Set<ValeSalidaDeAlmacen>().Add(vale);
        foreach (var producto in vale.Productos)
        {
            var existencia = await _db.Set<ExistenciaAlmacen>().SingleOrDefaultAsync(e => e.Id == producto.ProductoId);
            if (existencia.ExistenciaEnAlmacen - producto.Cantidad < 0)
            {
                return false;
            }
            existencia.ExistenciaEnAlmacen -= producto.Cantidad;
            _db.Entry(existencia).State = EntityState.Modified;
            await _centroDeCostoService.DarEntrada(producto.Producto.Producto.ProductoId, vale.CentroDeCostoId, producto.Cantidad, producto.Producto.Producto.UnidadDeMedidaId, vale.UsuarioId);
            if (!producto.Producto.Producto.Producto.EsInventariable)
            {
                var tipo =
                    await _db.Set<TipoDeMovimiento>()
                        .SingleOrDefaultAsync(t => t.Descripcion == TipoDeMovimientoConstantes.SalidaAProduccion);
                await _centroDeCostoService.DarSalida(producto.Producto.Producto.ProductoId, vale.CentroDeCostoId, tipo.Id,
                    producto.Cantidad, producto.Producto.Producto.UnidadDeMedidaId, vale.UsuarioId);
            }
            if (producto.Producto.Producto.Producto.EsInventariable && producto.Producto.Producto.ProporcionDeMerma > 0)
            {
                var tipo =
                    await _db.Set<TipoDeMovimiento>()
                        .SingleOrDefaultAsync(t => t.Descripcion == TipoDeMovimientoConstantes.Merma);
                var cantidadAMerma = producto.Cantidad * producto.Producto.Producto.ProporcionDeMerma;
                await _centroDeCostoService.DarSalida(producto.Producto.Producto.ProductoId, vale.CentroDeCostoId, tipo.Id,
                    cantidadAMerma, producto.Producto.Producto.UnidadDeMedidaId, vale.UsuarioId, "Merma automatica por traslado de almacen a centro de costo");
            }
        }
        await _db.SaveChangesAsync();
        return true;

    }

    /// <summary>
    /// Valida si se puede dar salida por merma
    /// </summary>
    /// <param name="salida"></param>
    /// <returns></returns>
    public async Task<bool> SePuedeDarSalidaPorMerma(SalidaPorMerma salida)
    {
        var existencia = await _db.Set<ExistenciaAlmacen>().FindAsync(salida.ExistenciaAlmacenId);
        var unidad = await _db.Set<UnidadDeMedida>().FindAsync(salida.UnidadDeMedidaId);
        var cantidadAExtraer = salida.Cantidad *
                               (existencia.Producto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
        if (existencia.ExistenciaEnAlmacen < cantidadAExtraer)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Dar salida por merma (salva en BD)
    /// </summary>
    /// <param name="detalleMerma"></param>
    /// <returns></returns>
    public async Task<bool> DarSalidaPorMerma(ICollection<SalidaPorMerma> detalleMerma)
    {
        var diaContable = await _periodoContableService.GetDiaContableActual();
        foreach (var detalle in detalleMerma)
        {
            detalle.DiaContableId = diaContable.Id;
            detalle.Fecha = DateTime.Now;

            var existencia = await _db.Set<ExistenciaAlmacen>().FindAsync(detalle.ExistenciaAlmacenId);
            var unidad = await _db.Set<UnidadDeMedida>().FindAsync(detalle.UnidadDeMedidaId);
            var cantidadAExtraer = detalle.Cantidad *
                                   (existencia.Producto.UnidadDeMedida.FactorDeConversion / unidad.FactorDeConversion);
            var costo = existencia.Producto.PrecioUnitario * cantidadAExtraer;
            detalle.Costo = costo;
            _db.Set<SalidaPorMerma>().Add(detalle);
            if (existencia.ExistenciaEnAlmacen < cantidadAExtraer)
            {
                return false;
            }
            existencia.ExistenciaEnAlmacen -= cantidadAExtraer;
            _db.Entry(existencia).State = EntityState.Modified;
        }
        await _db.SaveChangesAsync();
        return true;
    }


}
