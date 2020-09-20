using System;

namespace S3ZipSharp.Models
{
    /// <summary>
    /// Configuration for S3ZipSharp
    /// </summary>
    public class Config
    {
        /// <summary>
        /// AWS access key
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// AWS secret key
        /// </summary>
        public string SecretAccessKey { get; set; }
        /// <summary>
        /// AWS region
        /// </summary>
        public string AwsRegion { get; set; }
        /// <summary>
        /// Amount of objects to download an zip in parallel, reduce to improve memory footprint
        /// </summary>
        public int BatchSize { get; set; }
        /// <summary>
        /// Location of the temp zip file
        /// </summary>
        public string TempZipPath { get { return $"{TempZipDir}\\test.zip"; } }
        /// <summary>
        /// Path to the temp zip file
        /// </summary>
        public string TempZipDir { get; set; } = $"{System.IO.Path.GetTempPath()}\\S3ZipSharp\\{new Random().Next(10000, 99999)}";
    }
}
