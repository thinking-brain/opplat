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


namespace opplatApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>  
            {  
                // var config = new ConfigurationBuilder()  
                //     .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: false)  
                //     .Build();  
                //options.Listen(IPAddress.Loopback, config.GetValue<int>("Host:Port"));  
                options.Listen(IPAddress.Loopback, 5200);  
            }) .UseStartup<Startup>();
    }
}
