using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Opplat.MainApp.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OpplatDbContext>
{
    public OpplatDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OpplatDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=opplat-design;Trusted_Connection=True;");
        return new OpplatDbContext(optionsBuilder.Options, null);
    }
}
