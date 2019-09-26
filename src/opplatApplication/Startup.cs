using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Account.WebApi.Data;
using Account.WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using opplatApplication.Data;
using opplatApplication.Utils;
using Swashbuckle.AspNetCore.Swagger;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.VersatModels2;
using FinanzasWebApi.Helper;
using notificationsWebApi.Data;

[assembly: HostingStartup(typeof(opplatApplication.Startup))]
namespace opplatApplication
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
            //contabilidad context
            services.AddDbContext<ContabilidadWebApi.Data.ApiDbContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("ContabilidadApiDbContext"), b => b.MigrationsAssembly("ContabilidadWebApi")));

            services.AddDbContext<VersatDbContext>(options =>
               options.UseSqlServer(context.Configuration.GetConnectionString("VersatConnection")));

            services.AddDbContext<VersatDbContext2>(options =>
               options.UseSqlServer(context.Configuration.GetConnectionString("Versat2Connection")));
            //

            //finanzas db context
            services.AddDbContext<FinanzasWebApi.Data.ApiDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("FinanzasApiDbContext"), b => b.MigrationsAssembly("FinanzasWebApi")));
            //

            // NotificationsDbContext
            services.AddDbContext<notificationsDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("notificationsApi"), b => b.MigrationsAssembly("notificationsWebApi")));
            //

            services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("AccountConnection"), b => b.MigrationsAssembly("Account.WebApi")));

            services.AddIdentity<Usuario, IdentityRole>()
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<AccountDbContext>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://microapp.cu",
                        ValidAudience = "https://microapp.cu",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Admin123*1234567890"))
                    };
                });

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "Account API",
                        Description = "Gestiona la autenticacion y autorizacion, asi como la gestion de usuarios.",
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

            services.AddDbContext<OpplatAppDbContext>(options =>
                options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("opplatApplication")));
            services.AddTransient<OpplatAppDbContext>();
            services.AddSingleton<MenuLoader>();
            services.AddSingleton<LicenciaService>();

            //finanzas services
            services.AddSingleton<ObtenerPlanGI_Context>();
            services.AddSingleton<GetTotalIngresosEnMes>();
            services.AddSingleton<GetTotalEgresosEnMes>();
            //fin
            services.AddSpaStaticFiles(config =>
            {
                config.RootPath = context.Configuration.GetValue<string>("ClientApp");
            });

            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });
            var lista = new string[]{
                "Account.WebApi"
            };
            var asseblies = AppDomain.CurrentDomain.GetAssemblies().Where(c => lista.Contains(c.FullName));
            foreach (var assembly in asseblies)
            {
                mvcBuilder.AddApplicationPart(assembly);
            }
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v1/docs.json", "Account API v1");
                c.RoutePrefix = "docs";
            });
            app.UseMvc(builder =>
            {
                builder.MapRoute(
                    name: "default",
                    template: "admin/{controller=Home}/{action=Index}/{id?}");

            });
            app.UseSpaStaticFiles();
            app.Map("/home", conf =>
            {
                conf.UseSpa(builder =>
                {
                    builder.Options.SourcePath = config.GetValue<string>("ClientApp");

                });
            });
            // app.UseSpa(builder =>
            // {
            //     builder.Options.SourcePath = "../opplat-vue/dist";

            // });
        }
    }
}