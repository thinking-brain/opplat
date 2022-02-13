using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Data;
using FinanzasWebApi.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using FinanzasWebApi.Helpers;

namespace FinanzasWebApi.Controllers
{
    [Route("finanzas/[controller]")]
    [ApiController]
    public class UploadReportController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public UploadReportController(FinanzasDbContext context)
        {
            _context = context;
        }

        [HttpPost, Route("UploadReport/")]
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