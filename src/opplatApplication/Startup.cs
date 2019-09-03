using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using opplatApplication.Data;
using opplatApplication.Utils;

namespace opplatApplication
{
    public class Startup
    {
        private IConfiguration _configuration { get; }
        private IHostingEnvironment _hostingEnvironment { get; }
        private readonly IList<ModuleInfo> _modules = new List<ModuleInfo>();
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _hostingEnvironment = environment;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //LoadInstalledModules();

            services.AddDbContext<OpplatAppDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("opplatApplication")));
            services.AddTransient<OpplatAppDbContext>();

            services.AddSingleton<MenuLoader>();
            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // var loader = new MicroserviceLoader();
            // loader.AddMicroservice("/");
            //var moduleInitializerInterface = typeof(IModuleInitializer);
            foreach (var module in _modules)
            {
                // Register controller from modules
                //mvcBuilder.AddApplicationPart(module.Assembly);

                // Register dependency in modules
                // var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IHostingStartup).IsAssignableFrom(x)).FirstOrDefault();
                // if (moduleInitializerType != null && moduleInitializerType != typeof(IHostingStartup))
                // {
                //     var moduleInitializer = (IHostingStartup)Activator.CreateInstance(moduleInitializerType);                    
                // }
            }

            // TODO: break down to new method in new class
            //var builder = new ContainerBuilder();
            // foreach (var module in GlobalConfiguration.Modules)
            // {
            //     builder.RegisterAssemblyTypes(module.Assembly).AsImplementedInterfaces();
            // }
            // foreach (var assembly in GlobalConfiguration.OtherAssemblies)
            // {
            //     builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            // }

            // builder.RegisterInstance(Configuration);
            // builder.RegisterInstance(_hostingEnvironment);
            // builder.Populate(services);
            // var container = builder.Build();
            // return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            //app.UseCookiePolicy();
            //app.Map("account", account => { account.UseMvc(); });
            app.UseMvc();
        }

        private void LoadInstalledModules()
        {
            //var section = _configuration.GetSection("Modules");
            var modulesPath = _configuration.GetValue<string>("Modules:ModulesPath");
            var moduleRootFolder = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, modulesPath));
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
