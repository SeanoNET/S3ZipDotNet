using System;
using System.ComponentModel.Design;
using System.Xml.Schema;

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
        public int BatchSize { get; set; } = 10;
        /// <summary>
        /// Location of the temp zip file
        /// </summary>
        public string TempZipPath { get { return $"{TempZipDir}\\test.zip"; } }
        /// <summary>
        /// Path to the temp zip file
        /// </summary>
        public string TempZipDir { get; set; } = $"{System.IO.Path.GetTempPath()}\\S3ZipSharp\\{new Random().Next(10000, 99999)}";

        internal Ionic.Zlib.CompressionLevel ZlibCompressionLevel;

        /// <summary>
        /// Compression level when zipping files
        /// </summary>
        public CompressionLevel CompressionLevel { 
            get {
                switch (ZlibCompressionLevel)
                {
                    case Ionic.Zlib.CompressionLevel.None:
                        return CompressionLevel.None;
                    case Ionic.Zlib.CompressionLevel.BestSpeed:
                        return CompressionLevel.BestSpeed;                 
                    case Ionic.Zlib.CompressionLevel.Default:
                        return CompressionLevel.Default;                  
                    case Ionic.Zlib.CompressionLevel.BestCompression:
                        return CompressionLevel.BestCompression;
                    default:
                        return CompressionLevel.Default;
                } 
            } 
            set {

                switch (value)
                {
                    case CompressionLevel.None:
                        ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.None;
                        break;
                    case CompressionLevel.BestSpeed:
                        ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
                        break;
                    case CompressionLevel.Default:
                        ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.Default;
                        break;
                    case CompressionLevel.BestCompression:
                        ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                        break;
                    default:
                        ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.Default;
                        break;
                }
            } 
        }       
    }
}
