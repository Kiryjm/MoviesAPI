using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var webHost = CreateHostBuilder(args).Build();

           //Automatically apply all pending migrations to database 
           using (var scope = webHost.Services.CreateScope())
           {
               var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
               context.Database.Migrate();
           }

           webHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //This method configures several configuration providers
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
