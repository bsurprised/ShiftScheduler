using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Onyx.ShiftScheduler.Infrastructure.Data;

namespace Onyx.ShiftScheduler.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    EnsureDataStorageIsReady(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseKestrel(c => c.AddServerHeader = false);
        }

        private static void EnsureDataStorageIsReady(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            context.Initialize();
        }
    }
}