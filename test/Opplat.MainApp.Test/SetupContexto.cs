using System;

namespace Opplat.MainApp.Test
{
    public class SetupContexto
    {
        // public static DbContext GetContextoBase()
        public static object GetContextoBase()
        {
            // DbContextOptions<OpplatAppDbContext> options = new DbContextOptionsBuilder<OpplatAppDbContext>()
            //     .UseInMemoryDatabase(Guid.NewGuid().ToString())
            //     .EnableSensitiveDataLogging()
            //     .Options;

            // var context = new OpplatAppDbContext(options);

            // context.Database.EnsureDeleted();

            // return context;
            throw new NotImplementedException();
        }
    }
}
