using NLog.Web;
using SE_kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .UseWindowsService()
    .UseNLog()
    .Build();

host.Run();
