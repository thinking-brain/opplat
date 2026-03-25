using System.IO.Compression;

namespace Opplat.MainApp.Models;

public class ModuleFileManager
{
    IConfiguration _configuration;
    public ModuleFileManager(IConfiguration config)
    {
        _configuration = config;
    }

    public void UnZipModule(string file, string moduleName)
    {
        var modulesDir = _configuration.GetValue<string>("ModulesDirectory") ?? throw new InvalidOperationException("ModulesDirectory configuration not found");
        var path = Path.Combine(modulesDir, moduleName);
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            directory.Create();
        }
        else
        {
            directory.Delete(true);
            directory.Create();
        }
        ZipFile.ExtractToDirectory(file, path);
        File.Delete(file);
    }

    public void RunModuleRequirements()
    {
        //todo: codigo para ejecutar los requisitos necesarios para carcar un modulo
    }

    public void ReloadApplication()
    {

    }
}
