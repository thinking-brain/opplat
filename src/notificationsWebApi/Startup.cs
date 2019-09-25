using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using notificationsWebApi.Data;
using notificationsWebApi.Hubs;
using Swashbuckle.AspNetCore.Swagger;

[assembly: HostingStartup(typeof(notificationsWebApi.Startup))]

namespace notificationsWebApi
{
    public class Startup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
            builder.Configure(Configure);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddDbContext<notificationsDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("notificationsContext"), b => b.MigrationsAssembly("notificationsWebApi")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR();
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v2", new Info
                    {
                        Version = "v1",
                        Title = "Notifications Api",
                        Description = "Api para las notificaciones",
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v1/docs.json", "Notifications Api v1");
                c.RoutePrefix = "docs";
            });

            app.UseCors(builder =>
   {
       builder.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
   });
            app.UseSignalR(routes =>
        {
            routes.MapHub<NotificationsHub>("/notiHub");
        });
            app.UseMvc();
        }
    }
}
