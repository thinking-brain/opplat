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
using FinanzasWebApi.Data;
using FinanzasWebApi.Helper;
using Microsoft.EntityFrameworkCore;

[assembly: HostingStartup(typeof(FinanzasWebApi.Startup))]

namespace FinanzasWebApi
{
    public class Startup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
            builder.Configure(Configure);
        }

        public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("finanzas_v1", new Info
                   {
                       Version = "v1",
                       Title = "Finanzas API",
                       Description = "Gestión Financiera dentro del Sistema OPPLAT.",
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
                   c.IncludeXmlComments(xmlPath);
               });
            services.AddScoped<FinanzasDbContext>();
            services.AddScoped<ObtenerPlanGI>();
            // services.AddSingleton<GetTotalIngresosEnMes>();
            // services.AddSingleton<GetTotalEgresosEnMes>();

            services.AddDbContext<FinanzasDbContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("FinanzasDbContext"), b => b.MigrationsAssembly("FinanzasWebApi")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseHttpsRedirection();
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/finanzas_v1/docs.json", "Finanzas API v1");
                c.RoutePrefix = "docs";
            });
            app.UseMvc();
        }
    }
}
