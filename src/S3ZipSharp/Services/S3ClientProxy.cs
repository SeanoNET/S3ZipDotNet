using Amazon.S3;
using Amazon.S3.Model;
using S3ZipSharp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public async IAsyncEnumerable<List<string>> ListObjectsAsStream(string bucketName, string keyPrefix, [EnumeratorCancellation] CancellationToken token)
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
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var keys = new List<string>();
                result = await _s3Client.ListObjectsV2Async(request, token);
                request.ContinuationToken = result.NextContinuationToken;
                Console.WriteLine($"Found {result.S3Objects.Count} objects");
                keys.AddRange(result.S3Objects.Select(o => o.Key));
                yield return keys;

            } while (result.IsTruncated);
        }

        public async IAsyncEnumerable<Models.S3Object> FetchObjectsAsStream(string bucketName, List<string> keys, [EnumeratorCancellation] CancellationToken token)
        {
            Console.WriteLine("Fetching files from S3");
            foreach (var key in keys)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

#pragma warning disable IDE0063 // Use simple 'using' statement
                using (var result = await _s3Client.GetObjectAsync(request, token))
#pragma warning restore IDE0063 // Use simple 'using' statement
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
