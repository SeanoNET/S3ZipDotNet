using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace S3ZipSharp.Example
{
    class Program
    {
       
        static async Task Main()
        {
            //Console.ReadKey();
            var stopwatch = Stopwatch.StartNew();

            var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            S3ZipSharp objectsZipper = new S3ZipSharp(new Models.Config(cfg.GetSection("Aws")["AccessKey"], cfg.GetSection("Aws")["SecretAccessKey"], cfg.GetSection("Aws")["Region"], "s3-zip-dotnet"));

            objectsZipper.filterOutFiles = (key) =>
            {
                return !key.Contains(".txt");
            };

            await objectsZipper.ZipBucket("store-test","test.zip", new System.Threading.CancellationToken());


            Console.WriteLine($"Finished in { stopwatch.ElapsedMilliseconds}ms");

        }
    }
}
