using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using Microsoft.Extensions.Logging;

namespace VL.Health.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                          .ConfigureWebHostDefaults(
                              webBuilder => webBuilder.UseStartup<Startup>())
                          .ConfigureLogging(logging =>
                          {
                              logging.AddApplicationInsights(Environment.GetEnvironmentVariable("ApplicationInsights_InstrumentationKey"));
                              logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                          });
        }
    }
}
