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
        [HttpPost ("/contratacion/Dictamenes/UploadFile")]
        public async Task<IActionResult> POST (IFormFile file, int ContratoId, string NumeroDeDictamen, string Observaciones,
            string FundamentosDeDerecho, string Consideraciones, string Recomendaciones, string Username, string OtrosSi) {
            var d = context.Dictamenes.FirstOrDefault (x => x.Numero == NumeroDeDictamen);
            if (d != null) {
                return BadRequest ("Ya hay un dictamen con este numero " + NumeroDeDictamen);
            }
            if (file != null) {
                var contrato = context.Contratos.FirstOrDefault (c => c.Id == ContratoId);

                var dictamen = new Dictamen {
                    ContratoId = ContratoId,
                    Numero = NumeroDeDictamen,
                    Observaciones = Observaciones,
                    FundamentosDeDerecho = FundamentosDeDerecho,
                    Consideraciones = Consideraciones,
                    Recomendaciones = Recomendaciones,
                    Username = Username,
                    FechaDictamen = DateTime.Now,
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

        // PUT contratacion/dictamenes/id
        [HttpPut]
        public async Task<IActionResult> PUT (IFormFile file, int ContratoId, string NumeroDeDictamen, string Observaciones,
            string FundamentosDeDerecho, string Consideraciones, string Recomendaciones, string Username, string OtrosSi, int Id) {
            var d = context.Dictamenes.FirstOrDefault (x => x.Id == Id);
            d.ContratoId = ContratoId;
            d.Consideraciones = Consideraciones;
            d.Observaciones = Observaciones;
            d.Recomendaciones = Recomendaciones;
            d.FechaDictamen = DateTime.Now;
            d.FundamentosDeDerecho = FundamentosDeDerecho;
            d.Numero = NumeroDeDictamen;
            d.Username = Username;
            d.OtrosSi = OtrosSi;
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
                d.FilePath = filePath;
            }
            context.Entry (d).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }
         private Dictionary<string, string> GetMimeTypes () {
            return new Dictionary<string, string> { { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
        //Get :contratacion/contratos/downloadFile
        [HttpGet ("/contratacion/dictamenes/DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile (int id) {
            var dictamen = context.Dictamenes.FirstOrDefault (c => c.Id == id);
            if (dictamen.FilePath != null) {
                var path = dictamen.FilePath;
                var memory = new MemoryStream ();
                using (var stream = new FileStream (path, FileMode.Open)) { await stream.CopyToAsync (memory); }
                memory.Position = 0;
                var ext = Path.GetExtension (path).ToLowerInvariant ();
                return File (memory, GetMimeTypes () [ext], Path.GetFileName (path));
            }
            return NotFound ($"No tiene un documento guardado");
        }
        // DELETE contratacion/dictamenes/id
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