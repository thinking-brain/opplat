using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ContratacionWebApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using RhWebApi.Data;

[assembly : HostingStartup (typeof (ContratacionWebApi.Startup))]
namespace ContratacionWebApi {
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
                c.SwaggerDoc ("Cont-v1", new Info { Title = "Contratación Web API", Version = "Cont-v1", Description = "Api de Contratación del Sistema OPPLAT" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine (AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments (xmlPath);
            });

            //contratacion db context
            services.AddDbContext<ContratacionDbContext> (options =>
                options.UseNpgsql (context.Configuration.GetConnectionString ("ContratacionDbContext"), b => b.MigrationsAssembly ("ContratacionWebApi")));

            services.AddDbContext<RhWebApiDbContext> (options =>
                options.UseNpgsql (context.Configuration.GetConnectionString ("RhWebApiDbContext")));

            services.AddScoped<ContratacionDbContext> ();

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
                c.SwaggerEndpoint ("/docs/Cont-v1/docs.json", "SGCont Web API v1");
                c.RoutePrefix = "docs";
            });
            app.UseHttpsRedirection ();
            app.UseCors (build => build.AllowAnyOrigin ().AllowAnyHeader ().AllowAnyMethod ().AllowCredentials ());
            app.UseMvc ();
        }
    }
}