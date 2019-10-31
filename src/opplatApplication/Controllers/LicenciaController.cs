using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using opplatApplication.Models;
using opplatApplication.Utils;
using System.IO;
using LicenceChecker;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using opplatApplication.ViewModels;
using opplatApplication.Data;

namespace opplatApplication.Controllers
{
    [Route("admin/[controller]")]
    [ApiController]
    public class LicenciaController : Controller
    {
        IHostingEnvironment _enviroment;
        DbContext _db;
        LicenciaService _licenciaService;
        public LicenciaController(IHostingEnvironment enviroment, OpplatAppDbContext context, LicenciaService licenciaService)
        {
            _enviroment = enviroment;
            _db = context;
            _licenciaService = licenciaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var licencia = _licenciaService.Licencia;
            if (licencia == null)
            {
                return BadRequest("La aplicacion no posee una licencia activa");
            }
            var path = _enviroment.ContentRootPath;
            var cl = new LicenceChecker.Checker(Path.Combine(_enviroment.ContentRootPath, "keys/"));
            var isCorrect = cl.CheckIntegrity(new LicenceChecker.Licence
            {
                Application = licencia.Aplicacion,
                ExpirationDate = licencia.Vencimiento,
                LicenceHash = licencia.Hash,
                Suscriptor = licencia.Subscriptor
            });
            if (!isCorrect)
            {
                return BadRequest("Su licencia esta corrupta. Contacte al proveedor del sistema.");
            }
            if (licencia.Vencimiento < DateTime.Now.AddDays(15))
            {
                return BadRequest("Su licencia esta vencida. Contacte al administrador.");
            }
            var result = new LicenciaVm
            {
                Subscriptor = licencia.Subscriptor,
                FechaVencimiento = String.Format("{0:dd/MM/yyy}", licencia.Vencimiento),
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm]IFormFile licence)
        {
            var path = _enviroment.ContentRootPath;
            System.IO.File.Delete(Path.Combine(path, "licencia.lic"));
            using (var stream = System.IO.File.Create(Path.Combine(path, "licencia.lic")))
            {
                licence.CopyTo(stream);
            }
            var lic = LicenceLoader.LoadFromFile(Path.Combine(path, "licencia.lic"));

            var cl = new LicenceChecker.Checker(Path.Combine(_enviroment.ContentRootPath, "keys/"));
            if (cl.Check(lic, DateTime.Now))
            {
                _db.Set<Licencia>().RemoveRange(_db.Set<Licencia>().ToList());
                _db.Add(new Licencia
                {
                    Aplicacion = lic.Application,
                    Subscriptor = lic.Suscriptor,
                    Vencimiento = lic.ExpirationDate,
                    Hash = lic.LicenceHash
                });
                _db.SaveChanges();
                return Ok(new LicenciaVm { Subscriptor = lic.Suscriptor, FechaVencimiento = String.Format("{0:dd/MM/yyy}", lic.ExpirationDate) });
            }
            return BadRequest("Error el agregar la licencia");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var result = _licenciaService.Eliminar();
            if (result)
            {
                return Ok("Licencia borrada correctamente.");
            }
            return BadRequest("Error eliminando la licencia, contacte al administrador.");
        }
    }
}