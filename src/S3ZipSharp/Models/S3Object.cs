using System.IO;

namespace S3ZipSharp.Models
{
    internal class S3Object
    {
        public string Key { get; set; }
        public Stream Content { get; set; }
    }
}
