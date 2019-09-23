using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : Controller
    {
        private readonly RhWebApiContext context;
        public AreasController(RhWebApiContext context)
        {
            this.context = context;
        }

        // GET api/areas
        [HttpGet]
        public IEnumerable<Area> GetAll()
        {                            
            return context.Area.ToList();            
        }       
       
      // GET: api/areas/Id
        [HttpGet("{id}", Name = "GetArea")]
        public IActionResult GetbyId(int id)
        {
            var area = context.Area.FirstOrDefault(s => s.Id == id);

            if (area == null)
            {
                return NotFound();
            }
            return Ok(area);
           
        }

        // POST api/areas
       [HttpPost]
        public IActionResult POST([FromBody] Area area)
        {            
            if (ModelState.IsValid)
            {
                context.Area.Add(area);
                context.SaveChanges();
                return new CreatedAtRouteResult("GetArea", new { id = area.Id });
            }
            return BadRequest(ModelState);
        }

        // PUT api/areas/id
       [HttpPut("{id}")]
        public IActionResult PUT([FromBody] Area area, int id)
        {
            if (area.Id != id)
            {
                return BadRequest(ModelState);

            }
            context.Entry(area).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        // DELETE api/areas/id
       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
              var area = context.Area.FirstOrDefault(s => s.Id == id);

            if (area.Id != id)
            {
                return NotFound();
            }
            context.Area.Remove(area);
            context.SaveChanges();
            return Ok(area);
       }
    }
}
