using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using S3ZipSharp.Models;
using S3ZipSharp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace S3ZipSharp
{
    public class S3ZipSharp
    {
        string tempDir = $"{System.IO.Path.GetTempPath()}\\S3ZipSharp\\{new Random().Next(10000, 99999)}";

        private readonly Config _config;
        private readonly S3ClientProxy _s3ClientProxy;
        private readonly ObjectZipper _objectZipper;
        public S3ZipSharp(Config config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this._s3ClientProxy = new S3ClientProxy(new Amazon.S3.AmazonS3Client(config.AccessKeyId, config.SecretAccessKey, RegionEndpoint.APSoutheast2));
            string _tempZipPath = $"{tempDir}\\test.zip";
            this._objectZipper = new ObjectZipper(_tempZipPath);
         
        }

        public async Task<bool> ZipBucket(string bucketName)
        {
           var token = new System.Threading.CancellationToken();
           var objects = await _s3ClientProxy.ListObjects(bucketName, "", token);

            for (int i = objects.Count-1; i > 0; i--)
            {
                var obj = await _s3ClientProxy.FetchObject(bucketName, objects[i], token);
                _objectZipper.ZipObject(obj.Key, obj.Data);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            

            return true;
        }
    }
}
