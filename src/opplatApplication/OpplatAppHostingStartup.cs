using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using opplatApplication.Data;
using opplatApplication.Utils;

[assembly: HostingStartup(typeof(opplatApplication.OpplatAppHostingStartup))]
namespace opplatApplication
{
    public class OpplatAppHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
            builder.Configure(Configure);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //LoadInstalledModules();

            services.AddDbContext<OpplatAppDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=opplat_app_db;User Id=sa;Password=Admin123*;MultipleActiveResultSets=true", b => b.MigrationsAssembly("opplatApplication")));
            services.AddTransient<OpplatAppDbContext>();

            services.AddSingleton<MenuLoader>();
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuer = false,
            //         ValidateAudience = false,
            //         ValidateLifetime = true,
            //         ValidateIssuerSigningKey = true,
            //         ValidIssuer = "https://microapp.cu",
            //         ValidAudience = "https://microapp.cu",
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Admin123*1234567890"))
            //     };
            // });
            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // var loader = new MicroserviceLoader();
            // loader.AddMicroservice("/");
            //var moduleInitializerInterface = typeof(IModuleInitializer);
            // foreach (var module in _modules)
            // {
            //     // Register controller from modules
            //     //mvcBuilder.AddApplicationPart(module.Assembly);

            //     // Register dependency in modules
            //     // var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IHostingStartup).IsAssignableFrom(x)).FirstOrDefault();
            //     // if (moduleInitializerType != null && moduleInitializerType != typeof(IHostingStartup))
            //     // {
            //     //     var moduleInitializer = (IHostingStartup)Activator.CreateInstance(moduleInitializerType);                    
            //     // }
            // }

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
        public void Configure(IApplicationBuilder app)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Error");
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     app.UseHsts();
            // }
            //app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            //app.UseCookiePolicy();
            //app.Map("account", account => { account.UseMvc(); });
            app.UseMvc();
        }
    }
}