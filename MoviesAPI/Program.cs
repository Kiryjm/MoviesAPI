using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //This method configures severa configuration providers
            Host.CreateDefaultBuilder(args)

                //Adding new configuration provider json file custom.json.
                //It will be the last provider in order and hence will have highest precedence for app
                //.ConfigureAppConfiguration((env, config) =>
                //    {
                //        config.AddJsonFile("custom.json", optional: true, reloadOnChange: true);
                //    })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
