

using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Models;

namespace Opplat.Infrastructure.Services;

public class LicenseResponse
{
    public bool Status {get;set;}
    public License? License {get;set;}
    public string Message {get;set;} = String.Empty;
}

public class LicenseService
{
    private readonly DbContext _db;
    // private readonly IWebHostEnvironment _env;

    // public LicenseService(OpplatDbContext context, IWebHostEnvironment enviroment)
    // {
    //     _db = context;
    //     _env = enviroment;
    // }

    public async Task<LicenseResponse> GetLicense()
    {
        var license = await _db.Set<License>().FirstOrDefaultAsync();
        if (license == null)
        {
            return new LicenseResponse { Status = false, Message = "The application does not have an active license."};
        }
        // TODO: Re-enable LicenceChecker when package is available
        // var path = _env.ContentRootPath;
        // var cl = new LicenceChecker.Checker(Path.Combine(_env.ContentRootPath, "keys/"));
        // var isCorrect = cl.CheckIntegrity(new LicenceChecker.Licence
        // {
        //     Application = license.Application,
        //     ExpirationDate = license.ExpirationDate,
        //     LicenceHash = license.Hash,
        //     Suscriptor = license.Subscriber
        // });
        // if (!isCorrect)
        // {
        //     return new LicenseResponse { Status = false, Message = "Your license is corrupted. Contact the system provider."};
        // }
        if (license.ExpirationDate < DateTime.Now.AddDays(15))
        {
            return new LicenseResponse { Status = false, Message = "Your license has expired. Contact the administrator."};
        }
        return new LicenseResponse { Status = true, License = license, Message = "License is valid."};
    }

    // public async Task<LicenseResponse> AddLicense(IFormFile licence)
    // {
    //     // TODO: Re-enable LicenceChecker when package is available
    //     return new LicenseResponse { Status = false, Message = "License functionality temporarily disabled during upgrade."};
        
    //     // License newLicence;
    //     // var path = _env.ContentRootPath;
    //     // System.IO.File.Delete(Path.Combine(path, "licencia.lic"));
    //     // using (var stream = System.IO.File.Create(Path.Combine(path, "licencia.lic")))
    //     // {
    //     //     licence.CopyTo(stream);
    //     // }
    //     // var lic = LicenceLoader.LoadFromFile(Path.Combine(path, "licencia.lic"));

    //     // var cl = new LicenceChecker.Checker(Path.Combine(_env.ContentRootPath, "keys/"));
    //     // if (cl.Check(lic, DateTime.Now))
    //     // {
    //     //     _db.Set<License>().RemoveRange(_db.Set<License>().ToList());
    //     //     newLicence = new License
    //     //     {
    //     //         Application = lic.Application,
    //     //         Subscriber = lic.Suscriptor,
    //     //         ExpirationDate = lic.ExpirationDate,
    //     //         Hash = lic.LicenceHash
    //     //     };
    //     //     _db.Add(newLicence);
    //     //     await _db.SaveChangesAsync();
    //     //     return new LicenseResponse { Status = true, License = newLicence, Message = "Added successfully."};
    //     // }
    //     // return new LicenseResponse { Status = false, Message = "Error adding the license"};
    // }

    public async Task<bool> Delete()
    {
        // var license = await _db.Set<License>().SingleOrDefaultAsync();
        // if (license == null)
        // {
        //     return false;
        // }
        // var path = _env.ContentRootPath;
        // System.IO.File.Delete(Path.Combine(path, "licencia.lic"));
        // _db.Remove(license);
        // await _db.SaveChangesAsync();
        return true;
    }

}
