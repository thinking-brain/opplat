using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ImportadorDatos.HostedServices;
using ImportadorDatos.Jobs;
using ImportadorDatos.Models.EnlaceVersat;
using ImportadorDatos.Models.Versat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhWebApi.Data;
using ContratacionWebApi.Data;

namespace ImportadorDatos {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<VersatDbContext> (options =>
                options.UseSqlServer (Configuration.GetConnectionString ("VersatConnection")));

            services.AddDbContext<ContabilidadDbContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("ContabilidadDbContext")));

            services.AddDbContext<EnlaceVersatDbContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("EnlaceVersatDbContext")));

            services.AddDbContext<RhWebApiDbContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("RhWebApiDbContext")));

            services.AddDbContext<ContratacionDbContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("ContratacionDbContext")));

            services.AddScoped<VersatDbContext> ();
            services.AddScoped<ContabilidadDbContext> ();
            services.AddScoped<EnlaceVersatDbContext> ();
            services.AddScoped<RhWebApiDbContext> ();
            services.AddTransient<ImportarVersat> ();

            services.AddHostedService<UpdateCuentasHostedService> ();
            services.AddHostedService<UpdatePeriodosHostedService> ();
            services.AddHostedService<UpdateAsientosHostedService> ();

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            //todo: agregar job para importar cuentas que no de problemas de concurrencia
            //RecurringJob.AddOrUpdate(() => ImportarVersat.ImportarCuentasAsync(), Cron.Daily);
            app.UseHttpsRedirection ();
            app.UseMvc ();
        }
    }
}