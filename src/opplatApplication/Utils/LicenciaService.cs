using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using opplatApplication.Data;
using opplatApplication.Models;
using opplatApplication.ViewModels;

namespace opplatApplication.Utils
{
    public class LicenciaService
    {
        private static LicenciaService _service;
        private Licencia _licencia;

        private LicenciaService()
        {

        }

        public static LicenciaService GetService()
        {
            if (_service == null)
            {
                _service = new LicenciaService();
            }
            return _service;
        }

        public Licencia Licencia
        {
            get
            {
                if (_licencia == null)
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                    configurationBuilder.AddJsonFile("./appsettings.json");
                    IConfiguration configuration = configurationBuilder.Build();
                    DbContextOptions<OpplatAppDbContext> options = new DbContextOptionsBuilder<OpplatAppDbContext>()
                        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("opplatApplication")).Options;
                    using (var db = new OpplatAppDbContext(options))
                    {
                        _licencia = db.Set<Licencia>().FirstOrDefault();
                    }
                }
                return _licencia;
            }
        }

    }
}