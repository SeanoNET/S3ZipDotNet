using Amazon.S3;
using Amazon.S3.Model;
using S3ZipSharp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace S3ZipSharp.Services
{
    internal class S3ClientProxy : IFileRetriever
    {
        private readonly AmazonS3Client _s3Client;
        private readonly int batchSize;

        public S3ClientProxy(AmazonS3Client s3Client, int batchSize)
        {
            if (s3Client is null)
            {
                throw new ArgumentNullException(nameof(s3Client));
            }

            this._s3Client = s3Client;
            this.batchSize = batchSize;
        }
        public async IAsyncEnumerable<List<string>> ListObjectsAsStream(string bucketName, string keyPrefix, CancellationToken token)
        {
            Console.WriteLine("Listing objects in S3");
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = keyPrefix,
                MaxKeys = batchSize
            };

            ListObjectsV2Response result;
            do
            {

                var keys = new List<string>();
                result = await _s3Client.ListObjectsV2Async(request, token);
                request.ContinuationToken = result.NextContinuationToken;
                Console.WriteLine($"Found {result.S3Objects.Count} objects");
                keys.AddRange(result.S3Objects.Select(o => o.Key));
                yield return keys;

            } while (result.IsTruncated);
        }

        public async IAsyncEnumerable<Models.S3Object> FetchObjectsAsStream(string bucketName, List<string> keys, CancellationToken token)
        {
            Console.WriteLine("Fetching files from S3");
            foreach (var key in keys)
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                using (var result = await _s3Client.GetObjectAsync(request, token))
                {
                    Console.WriteLine($"Downloaded {key}");
                    yield return new Models.S3Object()
                    {
                        Key = result.Key,
                        Content = result.ResponseStream
                    };
                }
            }

        }
    }
}
