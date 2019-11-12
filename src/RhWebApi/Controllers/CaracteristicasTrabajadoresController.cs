using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;
using RhWebApi.Data;

namespace RhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaracteristicasTrabController : Controller
    {
        private readonly RhWebApiDbContext context;
        public CaracteristicasTrabController(RhWebApiDbContext context)
        {
            this.context = context;
        }

        // GET api/CaracteristicasTrab
        [HttpGet]
        public IEnumerable<CaracteristicasTrab> GetAll()
        {                            
            return context.CaracteristicasTrab.ToList();            
        }       
       
      // GET: api/CaracteristicasTrab/Id
        [HttpGet("{id}", Name = "GetCaracteristicasTrab")]
        public IActionResult GetbyId(int id)
        {
            var caractTrab = context.CaracteristicasTrab.FirstOrDefault(s => s.TrabajadorId == id);

            if (caractTrab == null)
            {
                return NotFound();
            }
            return Ok(caractTrab);
           
        }

        // POST api/CaracteristicasTrab
       [HttpPost]
        public IActionResult POST([FromBody] CaracteristicasTrab caractTrab)
        {            
            if (ModelState.IsValid)
            {
                context.CaracteristicasTrab.Add(caractTrab);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetCaracteristicasTrab", new { id = caractTrab.TrabajadorId });
            }
            return BadRequest(ModelState);
        }

        // PUT api/CaracteristicasTrab/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] CaracteristicasTrab caractTrab, int id)
        {
            if (caractTrab.TrabajadorId != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(caractTrab).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE api/CaracteristicasTrab/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var caractTrab = context.CaracteristicasTrab.FirstOrDefault(s => s.TrabajadorId == id);

            if (caractTrab.TrabajadorId != id)
            {
                return NotFound();
            }
            context.CaracteristicasTrab.Remove(caractTrab);
            context.SaveChanges();
            return Ok(caractTrab);
       }
    }
}
