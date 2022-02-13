using LicenceChecker;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Utils;

public class LicenciaResponse
{
    public bool Status {get;set;}
    public Licencia? Licencia {get;set;}
    public string Mensaje {get;set;} = String.Empty;
}

public class LicenciaService
{
    private readonly DbContext _db;
    private readonly IWebHostEnvironment _env;

    public LicenciaService(OpplatDbContext context, IWebHostEnvironment enviroment)
    {
        _db = context;
        _env = enviroment;
    }

    public async Task<LicenciaResponse> GetLicencia()
    {
        var licencia = await _db.Set<Licencia>().FirstOrDefaultAsync();
        if (licencia == null)
        {
            return new LicenciaResponse { Status = false, Mensaje = "La aplicacion no posee una licencia activa."};
        }
        var path = _env.ContentRootPath;
        var cl = new LicenceChecker.Checker(Path.Combine(_env.ContentRootPath, "keys/"));
        var isCorrect = cl.CheckIntegrity(new LicenceChecker.Licence
        {
            Application = licencia.Aplicacion,
            ExpirationDate = licencia.Vencimiento,
            LicenceHash = licencia.Hash,
            Suscriptor = licencia.Subscriptor
        });
        if (!isCorrect)
        {
            return new LicenciaResponse { Status = false, Mensaje = "Su licencia esta corrupta. Contacte al proveedor del sistema."};
        }
        if (licencia.Vencimiento < DateTime.Now.AddDays(15))
        {
            return new LicenciaResponse { Status = false, Mensaje = "Su licencia esta vencida. Contacte al administrador."};
        }
        return new LicenciaResponse { Status = true,Licencia = licencia, Mensaje = "Licencia correcta."};
    }

    public async Task<LicenciaResponse> AddLicencia(IFormFile licence)
    {
        Licencia newLicence;
        var path = _env.ContentRootPath;
        System.IO.File.Delete(Path.Combine(path, "licencia.lic"));
        using (var stream = System.IO.File.Create(Path.Combine(path, "licencia.lic")))
        {
            licence.CopyTo(stream);
        }
        var lic = LicenceLoader.LoadFromFile(Path.Combine(path, "licencia.lic"));

        var cl = new LicenceChecker.Checker(Path.Combine(_env.ContentRootPath, "keys/"));
        if (cl.Check(lic, DateTime.Now))
        {
            _db.Set<Licencia>().RemoveRange(_db.Set<Licencia>().ToList());
            newLicence = new Licencia
            {
                Aplicacion = lic.Application,
                Subscriptor = lic.Suscriptor,
                Vencimiento = lic.ExpirationDate,
                Hash = lic.LicenceHash
            };
            _db.Add(newLicence);
            await _db.SaveChangesAsync();
            return new LicenciaResponse { Status = true, Licencia = newLicence, Mensaje= "Agregado correctamente."};
        }
        return new LicenciaResponse { Status = false, Mensaje= "Error el agregar la licencia"};
    }

    public async Task<bool> Eliminar()
    {
        var licencia = await _db.Set<Licencia>().SingleOrDefaultAsync();
        if (licencia == null)
        {
            return false;
        }
        var path = _env.ContentRootPath;
        System.IO.File.Delete(Path.Combine(path, "licencia.lic"));
        _db.Remove(licencia);
        await _db.SaveChangesAsync();
        return true;
    }

}
