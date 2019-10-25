using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioWebApi.Controllers
{
    [Route("inventario/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public ProductosController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Producto>> Get()
        {
            return _context.Productos.Include(p => p.Unidad).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Almacen> Get(int id)
        {
            var almacen = _context.Almacenes.Find(id);
            if (almacen != null)
            {
                return Ok(almacen);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] Almacen almacen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(almacen);
                _context.SaveChanges();
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
            var almacen = _context.Almacenes.Find(id);
            if(almacen != null){
                _context.Remove(almacen);
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}