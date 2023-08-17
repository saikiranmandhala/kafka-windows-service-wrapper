using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace KafkaWindowsServiceWrapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("init main function started");
                CreateHostBuilder(args).Build().Run();
                logger.Info("init main function completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = ServiceName(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog();

        private static string ServiceName(string[] args)
        {
            var q = from arg in args
                    where arg.StartsWith("--name=")
                    select arg.Split("=")[1];
            return q.Single();
        }
    }
}
