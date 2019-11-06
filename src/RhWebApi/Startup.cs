using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using RhWebApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;

[assembly: HostingStartup(typeof(RhWebApi.Startup))]
namespace RhWebApi
{
    public class Startup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
            builder.Configure(Configure);
        }

        public void ConfigureConfigurationBuilder(WebHostBuilderContext context)
        {

        }

        public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("rh-v1", new Info { Title = "RH Web API", Version = "rh_v1", Description = "Api de Recursos Humanos del Sistema OPPLAT" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<RhWebApiDbContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("RhWebApiDbContext"), b => b.MigrationsAssembly("RhWebApi")));

            // services.AddDbContext<VersatDbContext>(options =>
            //    options.UseSqlServer(context.Configuration.GetConnectionString("VersatConnection")));

            // services.AddDbContext<VersatDbContext2>(options =>
            //    options.UseSqlServer(context.Configuration.GetConnectionString("Versat2Connection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/rh-v1/docs.json", "RH Web API v1");
                c.RoutePrefix = "docs";
            });
            app.UseHttpsRedirection();
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseMvc();
        }
    }
}