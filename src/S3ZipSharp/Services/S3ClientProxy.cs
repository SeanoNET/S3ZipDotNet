using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using S3ZipSharp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp.Services
{
    public class S3ClientProxy : IFileRetriever
    {
        private readonly AmazonS3Client _s3Client;
        private readonly int batchSize;
        private readonly ILogger _logger;

        public S3ClientProxy(AmazonS3Client s3Client, int batchSize, ILogger logger)
        {
            if (s3Client is null)
            {
                throw new ArgumentNullException(nameof(s3Client));
            }

            this._s3Client = s3Client;
            this.batchSize = batchSize;
            this._logger = logger;
        }
        public async IAsyncEnumerable<List<string>> ListObjectsAsStream(string bucketName, string folderName, [EnumeratorCancellation] CancellationToken token)
        {
            _logger?.LogTrace($"Listing objects in bucket {bucketName}");

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = folderName + "/",
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
                _logger?.LogTrace($"Found {result.S3Objects.Count} objects");
                keys.AddRange(result.S3Objects.Select(o => o.Key));
                yield return keys;

            } while (result.IsTruncated);
        }

        public async IAsyncEnumerable<Models.S3Object> FetchObjectsAsStream(string bucketName, string folderName, List<string> keys, [EnumeratorCancellation] CancellationToken token)
        {
            _logger?.LogTrace("Fetching files from S3");
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
                    _logger?.LogTrace($"Downloaded {key}");
                    yield return new Models.S3Object()
                    {
                        Key = result.Key,
                        Content = result.ResponseStream
                    };
                }
            }

        }

        public Task UploadZipAsync(string zipFilePath, string bucketName, string s3ZipFileName)
        {
            _logger?.LogTrace($"Uploading zip file {zipFilePath} into {bucketName}/{s3ZipFileName}");
            var fileTransferUtility = new TransferUtility(_s3Client);
            return fileTransferUtility.UploadAsync(zipFilePath, bucketName, s3ZipFileName);
        }
    }
}
