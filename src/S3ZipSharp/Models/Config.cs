using System;
using System.Collections.Specialized;
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
        public readonly string AccessKeyId;
        /// <summary>
        /// AWS secret key
        /// </summary>
        public readonly string SecretAccessKey;
        /// <summary>
        /// AWS region
        /// </summary>
        public readonly string AwsRegion;
        /// <summary>
        /// Name of the S3 bucket
        /// </summary>
        public readonly string S3BucketName;
        /// <summary>
        /// Amount of objects to download an zip in parallel, reduce to improve memory footprint
        /// </summary>
        public readonly int BatchSize = 10;
        /// <summary>
        /// Location of the temp zip file
        /// </summary>
        public readonly string TempZipPath;
        /// <summary>
        /// Path to the temp zip file
        /// </summary>
        public readonly string TempZipDir;

        internal Ionic.Zlib.CompressionLevel ZlibCompressionLevel = Ionic.Zlib.CompressionLevel.Default;

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
        public Config(
            string accessKeyId,
            string secretAccessKey,
            string awsRegion,
            string s3BucketName
            ) : this(accessKeyId, secretAccessKey, awsRegion, s3BucketName, 10,"","")
        {

        }

        public Config(
            string accessKeyId,
            string secretAccessKey,
            string awsRegion,
            string s3BucketName,
            int batchSize,
            string tempZipPath,
            string tempZipDir
            )
        {

            if (String.IsNullOrEmpty(s3BucketName))
                throw new ArgumentNullException($"Must provide {nameof(S3BucketName)}");

            if (String.IsNullOrEmpty(tempZipDir))
                tempZipDir = $"{System.IO.Path.GetTempPath()}\\S3ZipSharp\\{new Random().Next(10000, 99999)}";

            if (String.IsNullOrEmpty(tempZipPath))
                tempZipPath = $"{tempZipDir}\\test.zip";
       
            AccessKeyId = accessKeyId;
            SecretAccessKey = secretAccessKey;
            AwsRegion = awsRegion;
            S3BucketName = s3BucketName;
            BatchSize = batchSize;
            TempZipPath = tempZipPath;
            TempZipDir = tempZipDir;
        }
    }
}
