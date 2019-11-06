using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using InventarioWebApi.Services.InventarioService;
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
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPost("/entrada")]
        public async Task<IActionResult> Entrada([FromBody] MovimientoDeProducto movimiento)
        {
            movimiento.TipoMovimientoId = 1;

            if (ModelState.IsValid)
            {
                _context.MovimientosDeProductos.Add(movimiento);
                await _context.SaveChangesAsync();
                await _inventario.ActualizarSubmayor(movimiento.Id);
                return Ok();
            }
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
