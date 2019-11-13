using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventarioWebApi.Controllers
{
    [Route("inventario/[controller]")]
    [ApiController]
    public class TiposDeMovimientoController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public TiposDeMovimientoController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<TipoMovimiento> Get()
        {
            return _context.TiposDeMovimiento;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<TipoMovimiento> Get(int id)
        {
            var tm = _context.TiposDeMovimiento.Find(id);
            if (tm != null)
            {
                return Ok(tm);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] TipoMovimiento tm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tm);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TipoMovimiento tm)
        {
            if (ModelState.IsValid)
            {
                var t = await _context.TiposDeMovimiento.FindAsync(id);
                if (t != null)
                {
                    t.Descripcion = tm.Descripcion;
                    t.Factor = tm.Factor;
                    _context.Update(t);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var tm = _context.TiposDeMovimiento.Find(id);
            if (tm != null)
            {
                _context.Remove(tm);
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
