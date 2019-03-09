using System;
using Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            InitializeDb(host);

            host.Run();
        }

        private static void InitializeDb(IWebHost host)
        {
            try
            {
                var storeContextFactory = new StoreContextFactory();
                var context = storeContextFactory.CreateDbContext(null);
                context.Database.Migrate();
                SeedData.Initialize(context);
            }
            catch (Exception)
            {
                throw new Exception("Cannot initialize database.");
            }
        }


        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>();
    }
}
