using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using ContabilidadWebApi.Helpers;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class PlanGIController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public PlanGIController(ContabilidadDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve los Planes de IG
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PlanesGI()
        {
            var planes = _context.Set<PlanGI>().Include(p => p.Detalles)
            .ThenInclude(p => p.Concepto).Select(
                s => new
                {
                    id = s.Id,
                    nombre = s.Titulo,
                    year = s.Year
                }).ToList();
            return Ok(planes);
        }

        /// <summary>
        /// Eliminar Plan
        /// </summary>
        /// <returns></returns>
        // GET: api/DeletePlanesGI
        [HttpDelete("{id}")]
        public ActionResult DeletePlanesGI(int id)
        {
            var planes = _context.Set<PlanGI>().Find(id);
            _context.Set<PlanGI>().Remove(planes);
            _context.SaveChanges();
            return Ok();
        }



        [HttpPost, Route("UploadPlanGI/")]
        public async Task<IActionResult> FileUpload(IFormFile file, [FromForm]string year)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream).ConfigureAwait(false);
                try
                {

                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Tip: To access the first worksheet, try index 1, not 0
                        var planHelper = new ReadExcelHelper(_context);
                        planHelper.readExcelPackageToString(package, worksheet, year);

                    }
                    return Ok();
                }
                catch (System.Exception)
                {
                    return BadRequest();
                }

            }
        }
    }
}