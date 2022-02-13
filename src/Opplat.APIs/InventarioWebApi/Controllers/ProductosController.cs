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
    public class ProductosController : ControllerBase
    {
        private readonly InventarioDbContext _context;
        private readonly InventarioService _inventario;

        public ProductosController(InventarioDbContext context)
        {
            _context = context;
            _inventario = new InventarioService(context);
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Producto>> Get()
        {
            return _context.Productos.Include(p => p.Unidad).Include(p => p.Tipo).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Almacen> Get(int id)
        {
            var producto = _context.Productos.Include(p => p.Unidad)
            .Include(p => p.Categoria).Include(p => p.Tipo).FirstOrDefault(p => p.Id == id);

            if (producto != null)
            {
                return Ok(producto);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] Producto producto)
        {
            if (_context.Productos.Any(p => p.Codigo == producto.Codigo))
            {
                ModelState.AddModelError("Codigo", "Ya existe un producto con este códico");
            }
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        
        [HttpPost("transferir")]
        public async Task<IActionResult> Transferir([FromBody] TransferenciaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Este movimiento representa la salida del almacen de Origen
                MovimientoDeProducto salida = new MovimientoDeProducto()
                {
                    AlmacenId = viewModel.AlmacenOrigenId,
                    ProductoId = viewModel.ProductoId,
                    TipoMovimientoId = 10,
                    Cantidad = viewModel.Cantidad,
                    UnidadDeMedidaId = viewModel.UnidadDeMedidaId,
                };
                //Este otro la entrada en el almacen de destino
                MovimientoDeProducto entrada = new MovimientoDeProducto()
                {
                    AlmacenId = viewModel.AlmacenDestinoId,
                    ProductoId = viewModel.ProductoId,
                    TipoMovimientoId = 1,
                    Cantidad = viewModel.Cantidad,
                    UnidadDeMedidaId = viewModel.UnidadDeMedidaId,
                };

                var t1 = await _inventario.ActualizarSubmayor(
                    salida.AlmacenId,
                    salida.ProductoId,
                    salida.Cantidad,
                    salida.UnidadDeMedidaId,
                    salida.TipoMovimientoId
                );

                var t2 = await _inventario.ActualizarSubmayor(
                    entrada.AlmacenId,
                    entrada.ProductoId,
                    entrada.Cantidad,
                    entrada.UnidadDeMedidaId,
                    entrada.TipoMovimientoId
                );

                if (t1.IsSuccess && t2.IsSuccess)
                {
                    _context.MovimientosDeProductos.Add(salida);
                    _context.MovimientosDeProductos.Add(entrada);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                string[] errors = { t1.Message, t2.Message };
                return BadRequest(errors);
            };
            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto != null)
            {
                _context.Remove(producto);
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
