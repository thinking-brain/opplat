<<<<<<< HEAD
﻿using System;
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
<<<<<<< HEAD
=======
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using ContratacionWebApi.Data;

[assembly: HostingStartup(typeof(ContratacionWebApi.Startup))]
namespace ContratacionWebApi
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
<<<<<<< HEAD
                c.SwaggerDoc("SGCont-v1", new Info { Title = "SGCont Web API", Version = "SGCont_v1", Description = "Api de Contratación del Sistema OPPLAT" });
=======
                c.SwaggerDoc("Cont-v1", new Info { Title = "Contratación Web API", Version = "Cont_v1", Description = "Api de Contratación del Sistema OPPLAT" });
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

<<<<<<< HEAD
             //contratación db context
            services.AddDbContext<ContratacionDbContext> (options =>
                options.UseNpgsql (context.Configuration.GetConnectionString ("ContratacionDbContext"), b => b.MigrationsAssembly ("ContratacionWebApi")));
=======
            services.AddDbContext<ContratacionDbContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("ContratacionDbContext"), b => b.MigrationsAssembly("ContratacionWebApi")));
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c

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
<<<<<<< HEAD
                c.SwaggerEndpoint("/docs/SGCont-v1/docs.json", "SGCont Web API v1");
=======
                c.SwaggerEndpoint("/docs/Cont-v1/docs.json", "Contratacion Web API v1");
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
                c.RoutePrefix = "docs";
            });
            app.UseHttpsRedirection();
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseMvc();
        }
    }
}