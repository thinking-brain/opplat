using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using op_costos_api.Models;
using op_costos_api.VersatModels;
using op_costos_api.VersatModels2;
using Swashbuckle.AspNetCore.Swagger;

[assembly: HostingStartup(typeof(op_costos_api.Startup))]
namespace op_costos_api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("costos-v1", new Info { Title = "OP COSTOS API", Version = "v1", Description = "Api de Costos del Sistema OPPLAT" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<ApiDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("ApiDbContext")));

            services.AddDbContext<VersatDbContext>(options =>
               options.UseSqlServer(context.Configuration.GetConnectionString("VersatConnection")));

            services.AddDbContext<VersatDbContext2>(options =>
               options.UseSqlServer(context.Configuration.GetConnectionString("Versat2Connection")));
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
                c.SwaggerEndpoint("/docs/costos-v1/docs.json", "Costos API v1");
                c.RoutePrefix = "docs";
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}