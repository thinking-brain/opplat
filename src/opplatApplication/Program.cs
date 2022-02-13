using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using opplatApplication.Utils;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using notificationsWebApi.Data;
using opplatApplication.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using opplatApplication.Services;
using Account.WebApi.Data;
using Account.WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddDbContext<AccountDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountConnection"), b => b.MigrationsAssembly("Account.WebApi")));

builder.Services.AddDbContext<OpplatAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("opplatApplication")));

// builder.Services.AddIdentity<Usuario, IdentityRole>()
//     .AddEntityFrameworkStores<AccountDbContext>()
//     .AddDefaultTokenProviders();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = false,
//         ValidateAudience = false,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = "https://opplat.com",
//         ValidAudience = "https://opplat.com",
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecurityKey")))
//     };
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Version = "v1",
//         Title = "Opplat API",
//         Description = "Gestiona la autenticacion y autorizacion, asi como la gestion de usuarios.",
//         TermsOfService = new Uri("https://example.com/terms"),
//         Contact = new OpenApiContact
//         {
//             Name = "EFAVAI Tech",
//             Email = "efavai.tech@gmail.com",
//             Url = new Uri("https://efavai.com/")
//         }
//     });

//     // Set the comments path for the Swagger JSON and UI.
//     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//     c.IncludeXmlComments(xmlPath);
// });
// builder.Services.Configure<SmtpSettings>(context.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddTransient<OpplatAppDbContext>();
// builder.Services.AddTransient<AccountDbContext>();
builder.Services.AddSingleton<MenuLoader>();
builder.Services.AddSingleton<LicenciaService>();

// builder.Services.AddSignalR();

var mvcBuilder = builder.Services.AddOrchardCore().AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// app.UseAuthentication();
// app.UseIdentityServer();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.UseStaticFiles();
// app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/docs/v1/docs.json", "Opplat API v1");
//     c.RoutePrefix = "docs";
// });
app.UseOrchardCore();


// app.UseSignalR(routes =>
// {
//     routes.MapHub<NotificationsHub>("/notihub");
// });
// app.UseMvc(builder =>
// {
//     builder.MapRoute(
//         name: "default",
//         template: "admin/{controller=Home}/{action=Index}/{id?}");

// });
// app.UseSpaStaticFiles();
// app.Map("/home", conf =>
// {
//     conf.UseSpa(builder =>
//     {
//         builder.Options.SourcePath = config.GetValue<string>("ClientApp");

//     });
// });
app.UseCors("CorsPolicy");
app.Run();
