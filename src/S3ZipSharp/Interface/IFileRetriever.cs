using S3ZipSharp.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp.Interface
{
    public interface IFileRetriever
    {
        IAsyncEnumerable<S3Object> FetchObjectsAsStream(string bucketName, string folderName, List<string> keys, CancellationToken token);
        IAsyncEnumerable<List<string>> ListObjectsAsStream(string bucketName, string folderName, CancellationToken token);
        Task UploadZipAsync(string zipFilePath, string bucketName, string folderName);
    }
}