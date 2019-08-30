using LicenceChecker;
using Microsoft.EntityFrameworkCore;
using opplatApplication.Models;

namespace opplatApplication.Data
{
    public class OpplatAppDbContext : DbContext
    {
        public OpplatAppDbContext(DbContextOptions<OpplatAppDbContext> options) : base(options)
        {

        }

        public DbSet<Licencia> Licencias { get; set; }
    }
}