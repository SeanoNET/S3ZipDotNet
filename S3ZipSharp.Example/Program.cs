using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace S3ZipSharp.Example
{
    class Logger : Interface.ILogger
    {
        private readonly ILogger logger;

        public Logger(ILogger logger)
        {
            this.logger = logger;
        }
        public void LogTrace(string message)
        {
            logger.Information(message);
        }
    }
    class Program
    {
        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .CreateLogger();

            //Console.ReadKey();
            var stopwatch = Stopwatch.StartNew();

            var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            S3ZipSharp objectsZipper = new S3ZipSharp(
                new Models.Config(cfg.GetSection("Aws")["AccessKey"], cfg.GetSection("Aws")["SecretAccessKey"], 
                cfg.GetSection("Aws")["Region"], 
                "s3-zip-dotnet"),
                new Logger(Log.Logger));

            objectsZipper.filterOutFiles = (key) =>
            {
                return !key.Contains(".txt");
            };

            await objectsZipper.ZipBucket("store-test","test.zip", new System.Threading.CancellationToken());


            Log.Logger.Information($"Finished in { stopwatch.ElapsedMilliseconds}ms");

        }
    }
}
