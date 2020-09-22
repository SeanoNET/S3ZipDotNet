using System.IO;

namespace S3ZipSharp.Models
{
    public class S3Object
    {
        public string Key { get; set; }
        public Stream Content { get; set; }
    }
}
