using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace S3ZipSharp.Example
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            //Console.ReadKey();
            var stopwatch = Stopwatch.StartNew();

            var cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            S3ZipSharp objectsZipper = new S3ZipSharp(new Models.Config() { AccessKeyId = cfg.GetSection("Aws")["AccessKey"], SecretAccessKey = cfg.GetSection("Aws")["SecretAccessKey"], AwsRegion = cfg.GetSection("Aws")["Region"] });


            await objectsZipper.ZipBucket("s3-zip-dotnet", new System.Threading.CancellationToken());


            Console.WriteLine($"Finished in { stopwatch.ElapsedMilliseconds}ms");

        }
    }
}
