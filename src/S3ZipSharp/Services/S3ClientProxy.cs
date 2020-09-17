using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp.Services
{
    internal class S3ClientProxy
    {
        private readonly AmazonS3Client _s3Client;

        public S3ClientProxy(AmazonS3Client s3Client)
        {
            if (s3Client is null)
            {
                throw new ArgumentNullException(nameof(s3Client));
            }

            this._s3Client = s3Client;
        }

        public async Task<List<string>> ListObjects(string bucketName, string keyPrefix, CancellationToken token)
        {
            var objects = new List<string>();

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = keyPrefix,
                MaxKeys = 10
            };

            ListObjectsV2Response result;

            do
            {
                result = await _s3Client.ListObjectsV2Async(request, token);
                objects.AddRange(result.S3Objects.Select(o => o.Key));
                request.ContinuationToken = result.NextContinuationToken;
            } while (result.IsTruncated);

            return objects;
        }

        public async Task<Models.S3Object> FetchObject(string bucketName, string key, CancellationToken token)
        {
            var request = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = key
            };

            using (var result = await _s3Client.GetObjectAsync(request, token))
            {
                using (var ms = new MemoryStream())
                {
                    await result.ResponseStream.CopyToAsync(ms);
                    return new Models.S3Object()
                    {
                        Key = result.Key,
                        Data = ms.ToArray()
                    };
                }
                    
            }
        }
    }
}
