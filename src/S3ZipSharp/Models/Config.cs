using System;
using System.Collections.Generic;
using System.Text;

namespace S3ZipSharp.Models
{
    public class Config
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string AwsRegion { get; set; }
    }
}
