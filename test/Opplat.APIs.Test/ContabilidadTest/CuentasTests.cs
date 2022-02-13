using System;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ContabilidadTest
{
    public class CuentasTests
    {
        [Fact]
        public void CuentasHijas()
        {
            var options = new DbContextOptionsBuilder<ContabilidadDbContext>().UseNpgsql("Host=localhost;Database=op_contabilidad_api;Username=opplat;Password=Admin123*");
            var context = new ContabilidadDbContext(options.Options);

            var helper = new CuentasHelper(context);

            var hijas = helper.CuentasHijas(7030);

            Assert.Equal(104, hijas.Count);
        }
    }
}
