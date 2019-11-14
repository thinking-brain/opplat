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
using InventarioWebApi.Data;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;

namespace InventarioWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<InventarioDbContext>(options =>
             options.UseNpgsql(Configuration.GetConnectionString("InventarioConnection"), b => b.MigrationsAssembly("InventarioWebApi")));

            services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v5", new Info
        {
            Version = "v5",
            Title = "Inventario Api",
            Description = "Api para la gestión de inventario",
            TermsOfService = "APACHE 2.0",
            Contact = new Contact
            {
                Name = "EFAVAI Tech",
                Email = "efavai.tech@gmail.com",
                Url = "https://efavai.com/"
            }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);        
    });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/log-{Date}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v5/docs.json", "Inventario Api V1");
                c.RoutePrefix = "docs";
            });


            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
