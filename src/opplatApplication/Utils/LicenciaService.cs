using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using opplatApplication.Data;
using opplatApplication.Models;
using opplatApplication.ViewModels;

namespace opplatApplication.Utils
{
    public class LicenciaService
    {
        private static LicenciaService _service;
        private Licencia _licencia;

        IConfiguration _config;

        public LicenciaService(IConfiguration config)
        {
            _config = config;
        }

        // public static LicenciaService GetService()
        // {
        //     if (_service == null)
        //     {
        //         _service = new LicenciaService();
        //     }
        //     return _service;
        // }

        public Licencia Licencia
        {
            get
            {
                if (_licencia == null)
                {
                    DbContextOptions<OpplatAppDbContext> options = new DbContextOptionsBuilder<OpplatAppDbContext>()
                        .UseSqlServer(_config.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("opplatApplication")).Options;
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