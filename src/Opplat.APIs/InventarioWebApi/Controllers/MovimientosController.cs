using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using InventarioWebApi.Services.InventarioService;
using InventarioWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioWebApi.Controllers
{
    [Route("inventario/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly InventarioDbContext _context;
        private readonly InventarioService _inventario;

        public MovimientosController(InventarioDbContext context)
        {
            _context = context;
            _inventario = new InventarioService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var movimientos = await _context.MovimientosDeProductos
                .Include(m => m.Producto)
                .Include(m => m.UnidadDeMedida)
                .Include(m => m.Almacen)
                .Include(m => m.TipoMovimiento).ToListAsync();

            return Ok(movimientos);
        }

        /// <summary>
        /// Devuelve todos los movimientos de un producto
        /// </summary>
        /// <param name="productoId"></param>
        /// <returns>Listado con todos los movimientos de un producto</returns>

        [HttpGet("{productoId}")]
        public async Task<IActionResult> PorProducto([FromRoute] int productoId)
        {
            var movimientos = await _context.MovimientosDeProductos.Where(m => m.ProductoId == productoId)
                .Include(m => m.UnidadDeMedida.Descripcion)
                .Include(m => m.Almacen.Nombre)
                .Include(m => m.TipoMovimiento.Descripcion).Select(m => new
                {
                    Fecha = m.Fecha.ToShortDateString(),
                    Almacen = m.Almacen.Nombre,
                    Cantidad = m.Cantidad,
                    UnidadDeMedida = m.UnidadDeMedida.Descripcion,
                    TipoDeMovimiento = m.TipoMovimiento.Descripcion
                }).ToListAsync();

            return Ok(movimientos);
        }

        [HttpGet("almacen/{almacenId}")]
        public async Task<IActionResult> PorAlmacen([FromRoute] int almacenId)
        {
            var movimientos = await _context.MovimientosDeProductos.Where(m => m.AlmacenId == almacenId)
                .Include(m => m.UnidadDeMedida.Descripcion)
                .Include(m => m.Producto.Nombre)
                .Include(m => m.TipoMovimiento.Descripcion).Select(m => new
                {
                    Fecha = m.Fecha.ToShortDateString(),
                    Producto = m.Producto.Nombre,
                    Cantidad = m.Cantidad,
                    UnidadDeMedida = m.UnidadDeMedida.Descripcion,
                    TipoDeMovimiento = m.TipoMovimiento.Descripcion
                }).ToListAsync();

            return Ok(movimientos);
        }

        [HttpGet("almacen/{almacenId}/producto/{productoId}")]
        public async Task<IActionResult> PorAlmacenProducto([FromRoute] int almacenId, int productoId)
        {
            var movimientos = await _context.MovimientosDeProductos.Where(m => m.AlmacenId == almacenId && m.ProductoId == productoId)
                .Include(m => m.UnidadDeMedida.Descripcion)
                .Include(m => m.TipoMovimiento.Descripcion).Select(m => new
                {
                    Fecha = m.Fecha.ToShortDateString(),
                    Cantidad = m.Cantidad,
                    UnidadDeMedida = m.UnidadDeMedida.Descripcion,
                    TipoDeMovimiento = m.TipoMovimiento.Descripcion
                }).ToListAsync();

            return Ok(movimientos);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MovimientoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                MovimientoDeProducto movimiento = new MovimientoDeProducto()
                {
                    AlmacenId = viewModel.AlmacenId,
                    ProductoId = viewModel.ProductoId,
                    TipoMovimientoId = viewModel.TipoMovimientoId,
                    Cantidad = viewModel.Cantidad,
                    UnidadDeMedidaId = viewModel.UnidadDeMedidaId,
                };
                var t1 = await _inventario.ActualizarSubmayor(
                                    viewModel.AlmacenId,
                                    viewModel.ProductoId,
                                    viewModel.Cantidad,
                                    viewModel.UnidadDeMedidaId,
                                    viewModel.TipoMovimientoId
                                    );

                if (t1.IsSuccess)
                {
                    _context.MovimientosDeProductos.Add(movimiento);
                    await _context.SaveChangesAsync();
                    return Ok("Operacion realizada satisfactoriamente");
                }
                return BadRequest(t1.Message);
            };
            return BadRequest(ModelState);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Almacen> Details(int id)
        {
            var movimiento = _context.MovimientosDeProductos.Include(m => m.Producto)
            .FirstOrDefault(m => m.Id == id);

            if (movimiento != null)
            {
                return Ok(movimiento);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
