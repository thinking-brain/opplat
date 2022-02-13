using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TallerWebApi.Data;

[assembly : HostingStartup (typeof (TallerWebApi.Startup))]
namespace TallerWebApi {
    public class Startup : IHostingStartup {
        public void Configure (IWebHostBuilder builder) {
            builder.ConfigureServices (ConfigureServices);
            builder.Configure (Configure);
        }

        public void ConfigureConfigurationBuilder (WebHostBuilderContext context) {

        }

        public void ConfigureServices (WebHostBuilderContext context, IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("rh-v1", new Info { Title = "Gestión Talleres Web API", Version = "rh_v1", Description = "Api de Gestión de Talleres del Sistema OPPLAT" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine (AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments (xmlPath);
            });

            services.AddDbContext<TallerWebApiDbContext> (options =>
                options.UseNpgsql (context.Configuration.GetConnectionString ("TallerWebApiDbContext"), b => b.MigrationsAssembly ("TallerWebApi")));
            services.AddScoped<TallerWebApiDbContext> ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app) {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment> ();
            var config = app.ApplicationServices.GetRequiredService<IConfiguration> ();

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
            }
            app.UseSwagger (c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI (c => {
                c.SwaggerEndpoint ("/docs/rh-v1/docs.json", "TallerWebApi Web API v1");
                c.RoutePrefix = "docs";
            });
            app.UseHttpsRedirection ();
            app.UseCors (build => build.AllowAnyOrigin ().AllowAnyHeader ().AllowAnyMethod ().AllowCredentials ());
            app.UseMvc ();
        }
    }
}