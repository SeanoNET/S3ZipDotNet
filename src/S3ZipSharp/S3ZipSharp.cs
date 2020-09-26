using Amazon;
using S3ZipSharp.Interface;
using S3ZipSharp.Models;
using S3ZipSharp.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp
{
    public class S3ZipSharp
    {
        private readonly IFileRetriever _s3ClientProxy;
        private readonly Config config;

        /// <summary>
        /// Filter out files using the name of the file
        /// </summary>
        public Func<string, bool> filterOutFiles;
        
        public S3ZipSharp(Config config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this._s3ClientProxy = new S3ClientProxy(new Amazon.S3.AmazonS3Client(config.AccessKeyId, config.SecretAccessKey, RegionEndpoint.APSoutheast2), config.BatchSize);
            this.config = config;
        }
        /// <summary>
        /// Retrieves and zip objects from a s3 bucket and zip them up, uploading the zip file back into s3
        /// </summary>
        /// <param name="s3FolderName">Name of the S3 bucket</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ZipBucket(string s3FolderName, string s3ZipFileName, CancellationToken cancellationToken)
        {

            using (var objectZipper = new ObjectZipper(config.TempZipPath, config.ZlibCompressionLevel))
            {
                //Create temp zip file in zip directory
                objectZipper.CreateZip();

                List<Task> fetchObjectsTasks = new List<Task>();
                ConcurrentBag<IAsyncEnumerable<Models.S3Object>> cb = new ConcurrentBag<IAsyncEnumerable<Models.S3Object>>();

                // Create tasks for downloading objects
                await foreach (var keys in _s3ClientProxy.ListObjectsAsStream(config.S3BucketName, s3FolderName, cancellationToken))
                {
                    var filterKeys = new List<string>();
                    if (filterOutFiles == null)
                    {
                        filterKeys = keys;
                    }
                    else
                    {
                        filterKeys = keys.Where(filterOutFiles).ToList();
                    }


                    fetchObjectsTasks.Add(Task.Run(() => cb.Add(_s3ClientProxy.FetchObjectsAsStream(config.S3BucketName,s3FolderName, filterKeys, cancellationToken))));
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
                                objectZipper.ZipObject(obj.Key, obj.Content);

                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                        }
                    }));
                }
                Task.WaitAll(consumeTasks.ToArray(), cancellationToken);


                await _s3ClientProxy.UploadZipAsync(config.TempZipPath, config.S3BucketName, s3ZipFileName);
                return true;
            }
               
        }       
    }
}
