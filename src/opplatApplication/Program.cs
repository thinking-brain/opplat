using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using opplatApplication.Utils;
using System.Reflection;
using System.Runtime.Loader;

namespace opplatApplication
{
    public class Program
    {
        private static IList<ModuleInfo> _modules = new List<ModuleInfo>();
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
               .UseKestrel(options =>
           {
               options.Listen(IPAddress.Loopback, 5200);
           })
           .UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "op_costos_api;opplatApplication;Account.WebApi");
            return host;
        }
    }
}
