using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class DictamenesController : Controller {
        private readonly ContratacionDbContext context;
        private IHostingEnvironment _hostingEnvironment;

        public DictamenesController (ContratacionDbContext context, IHostingEnvironment environment) {
            this.context = context;
            _hostingEnvironment = environment;
        }
        // GET contratacion/Dictamenes
        [HttpGet]
        public IEnumerable<Dictamen> GetAll () {
            return context.Dictamenes.ToList ();
        }

        // GET: contratacion/Dictamenes/Id
        [HttpGet ("{id}", Name = "GetDictamenes")]
        public IActionResult GetbyId (int id) {
            var dictamen = context.Dictamenes.FirstOrDefault (s => s.Id == id);

            if (dictamen == null) {
                return NotFound ();
            }
            return Ok (dictamen);
        }

        // POST contratacion/Dictamenes
        [HttpPost ("/contratacion/contratos/Dictamenes/UploadFile")]
        public async Task<IActionResult> POST (IFormFile file, int ContratoId, string NumeroDeDictamen, string Observaciones,
            string FundamentosDeDerecho, string Consideraciones, string Recomendaciones, string Username, string OtrosSi) {
            if (file != null) {
                var contrato = context.Contratos.FirstOrDefault (c => c.Id == ContratoId);

                var dictamen = new Dictamen {
                    ContratoId = ContratoId,
                    NumeroDeDictamen = NumeroDeDictamen,
                    Observaciones = Observaciones,
                    FundamentosDeDerecho = FundamentosDeDerecho,
                    Consideraciones = Consideraciones,
                    Recomendaciones = Recomendaciones,
                    Username = Username,
                    FechaDictamen=DateTime.Now,
                    OtrosSi = OtrosSi
                };
                if (file != null) {
                    string folderName = Path.Combine (_hostingEnvironment.WebRootPath, "Upload Dictamenes");
                    string subFolder = System.IO.Path.Combine (folderName, "Prueba");
                    if (!Directory.Exists (subFolder)) {
                        System.IO.Directory.CreateDirectory (subFolder);
                    }
                    var filePath = Path.Combine (subFolder, file.FileName);
                    using (var fileStream = new FileStream (filePath, FileMode.Create)) {
                        await file.CopyToAsync (fileStream);
                    }
                    dictamen.FilePath = filePath;
                }
                context.Dictamenes.Add (dictamen);
                context.SaveChanges ();
                return Ok ();
            }
            return NotFound ($"El archivo es null");
        }

        // PUT contratacion/dictamen/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Dictamen dictamen, int id) {
            if (dictamen.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (dictamen).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/dictamen/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var dictamen = context.Dictamenes.FirstOrDefault (s => s.Id == id);

            if (dictamen.Id != id) {
                return NotFound ();
            }
            context.Dictamenes.Remove (dictamen);
            context.SaveChanges ();
            return Ok (dictamen);
        }
    }
}