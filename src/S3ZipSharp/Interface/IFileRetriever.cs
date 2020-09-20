using S3ZipSharp.Models;
using System.Collections.Generic;
using System.Threading;

namespace S3ZipSharp.Interface
{
    internal interface IFileRetriever
    {
        IAsyncEnumerable<S3Object> FetchObjectsAsStream(string bucketName, List<string> keys, CancellationToken token);
        IAsyncEnumerable<List<string>> ListObjectsAsStream(string bucketName, string keyPrefix, CancellationToken token);
    }
}