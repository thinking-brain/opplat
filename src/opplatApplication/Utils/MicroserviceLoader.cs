using System.Reflection;
using System.Runtime.Loader;
using Newtonsoft.Json;

namespace opplatApplication.Utils;

public class MicroserviceLoader
{
    public string ModuleRootPath { get; set; } = string.Empty;
    public MicroserviceRegistry Registry { get; set; }
    private readonly IList<ModuleInfo> modules = new List<ModuleInfo>();
    public MicroserviceLoader()
    {
        Registry = new MicroserviceRegistry();
    }
    //TODO: Este es una prueba
    public void InitialLoad()
    {
        var moduleRootFolder = new DirectoryInfo(ModuleRootPath);
        var moduleFolders = moduleRootFolder.GetDirectories();

        foreach (var moduleFolder in moduleFolders)
        {
            var binFolder = new DirectoryInfo(moduleFolder.FullName);
            if (!binFolder.Exists) { continue; }
            foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly))
            {
                Assembly assembly = null!;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                }
                catch (FileLoadException ex)
                {
                    if (ex.Message == "Assembly with same name is already loaded")
                    {
                        // Get loaded assembly
                        assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                    }
                    else
                    {
                        throw;
                    }
                }

                if (assembly.FullName!.Contains(moduleFolder.Name))
                {
                    //mcmaster plugin

                    modules.Add(new ModuleInfo { Name = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.FullName });
                }
                else
                {
                    //if (!GlobalConfiguration.OtherAssemblies.Contains(assembly))
                    //{
                    //    GlobalConfiguration.OtherAssemblies.Add(assembly);
                    //}
                }
            }
        }
    }

    public void AddMicroservice(string servicePath)
    {
        var folder = new DirectoryInfo(servicePath);
        if (!folder.Exists)
        {
            return;
        }
        //add service to registry
        using (StreamReader r = new StreamReader("service.json"))
        {
            string json = r.ReadToEnd();
            var microservice = JsonConvert.DeserializeObject<Microservice>(json);
            MicroserviceRegistry.Services.Add(microservice!);
        }
        //add module to list of module
        var module = new ModuleInfo();
        foreach (var file in folder.GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly))
        {

            Assembly assembly = null!;
            try
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
            }
            catch (FileLoadException ex)
            {
                if (ex.Message == "Assembly with same name is already loaded")
                {
                    // Get loaded assembly
                    assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                }
                else
                {
                    throw;
                }
            }


            if (assembly.FullName!.Contains(folder.Name))
            {
                module.Name = folder.Name;
                module.Assembly = assembly;
                module.Path = folder.FullName;
            }
        }
        GlobalConfiguration.Modules.Add(module);
        //run configure on service/module
        var startupType = module.Assembly.DefinedTypes.Where(x => x.FullName.Contains("Startup")).FirstOrDefault().AsType();
        // var startup = (Startup)Activator.CreateInstance(startupType);
    }

    public void RemoveMicroservice()
    {

    }
}
