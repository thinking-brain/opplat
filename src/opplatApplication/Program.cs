using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using opplatApplication.Utils;
using System.Reflection;
using System.Runtime.Loader;

namespace opplatApplication
{
    public class Program
    {
        private static IList<ModuleInfo> _modules = new List<ModuleInfo>();
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //LoadInstalledModules();
            var host = WebHost.CreateDefaultBuilder(args)
               .UseKestrel(options =>
           {
               options.Listen(IPAddress.Loopback, 5200);
           })
           .UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "opplatApplication;Account.WebApi");
            return host;
        }

        // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

        //     WebHost.CreateDefaultBuilder(args)
        //         .UseKestrel(options =>
        //     {
        //         options.Listen(IPAddress.Loopback, 5200);
        //     })
        //     .UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "opplatApplication;Account.WebApi");
        //.UseStartup<Startup>();

        public static void LoadInstalledModules()
        {
            //var section = _configuration.GetSection("Modules");
            var _configuration = new ConfigurationBuilder().AddJsonFile("./appsettings.json").Build();
            var modulesPath = _configuration.GetValue<string>("Modules:ModulesPath");
            var moduleRootFolder = new DirectoryInfo("/media/elvis/datos/proyectos/netcore/opplat/src/");
            //            var moduleRootFolder = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, modulesPath));
            var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var moduleFolder in moduleFolders)
            {
                var file_name = moduleFolder.Name + ".dll";
                var directorios = moduleFolder.EnumerateFiles(file_name, SearchOption.AllDirectories).ToList();
                var dllFile = directorios.FirstOrDefault(d => d.Name == file_name);
                if (dllFile == null)
                {
                    continue;
                }
                var binFolder = dllFile.Directory;
                //var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (binFolder == null || !binFolder.Exists)
                {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly))
                {
                    Assembly assembly = null;
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

                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        _modules.Add(new ModuleInfo { Name = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.FullName });
                    }
                    else
                    {
                        if (!GlobalConfiguration.OtherAssemblies.Contains(assembly))
                        {
                            GlobalConfiguration.OtherAssemblies.Add(assembly);
                        }
                    }
                }
            }

            GlobalConfiguration.Modules = _modules;
        }
    }
}
