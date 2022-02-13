using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;

namespace Opplat.MainApp.Test
{
    public class SetupContexto
    {
        // public static DbContext GetContextoBase()
        // {
        //     DbContextOptions<OpplatAppDbContext> options = new DbContextOptionsBuilder<OpplatAppDbContext>()
        //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //         .EnableSensitiveDataLogging()
        //         .Options;

        //     var context = new OpplatAppDbContext(options);

        //     context.Database.EnsureDeleted();

        //     return context;
        // }
    }
}
