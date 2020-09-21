using Amazon;
using S3ZipSharp.Interface;
using S3ZipSharp.Models;
using S3ZipSharp.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp
{
    public class S3ZipSharp
    {
        private readonly IFileRetriever _s3ClientProxy;
        private readonly IObjectZipper _objectZipper;

        public S3ZipSharp(Config config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this._s3ClientProxy = new S3ClientProxy(new Amazon.S3.AmazonS3Client(config.AccessKeyId, config.SecretAccessKey, RegionEndpoint.APSoutheast2), config.BatchSize);
            this._objectZipper = new ObjectZipper(config.TempZipPath);

        }
        /// <summary>
        /// Retrieves and zip objects from a s3 bucket and zip them up, uploading the zip file back into s3
        /// </summary>
        /// <param name="bucketName">Name of the S3 bucket</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ZipBucket(string bucketName, CancellationToken cancellationToken)
        {
            //Create temp zip file in zip directory
            _objectZipper.CreateZip();

            List<Task> fetchObjectsTasks = new List<Task>();
            ConcurrentBag<IAsyncEnumerable<Models.S3Object>> cb = new ConcurrentBag<IAsyncEnumerable<Models.S3Object>>();

            // Create tasks for downloading objects
            await foreach (var keys in _s3ClientProxy.ListObjectsAsStream(bucketName, "", cancellationToken))
            {                                
                fetchObjectsTasks.Add(Task.Run(() => cb.Add(_s3ClientProxy.FetchObjectsAsStream(bucketName, keys, cancellationToken))));             
            }

           
            Task.WaitAll(fetchObjectsTasks.ToArray(), cancellationToken);

            // Download and zip the objects
            List<Task> consumeTasks = new List<Task>();
            while (!cb.IsEmpty)
            {
                consumeTasks.Add(Task.Run(async () =>
                {
                    if (cb.TryTake(out IAsyncEnumerable<S3Object> s3Object))
                    {
                        await foreach (var obj in s3Object)
                        {
                            _objectZipper.ZipObject(obj.Key, obj.Content);

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }
                }));
            }
            Task.WaitAll(consumeTasks.ToArray(), cancellationToken);

            return true;
        }       
    }
}
