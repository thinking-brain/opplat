using System;

namespace Opplat.UnitTest
{
    public class SetupContext
    {
        // public static DbContext GetBaseContext()
        public static object GetBaseContext()
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
