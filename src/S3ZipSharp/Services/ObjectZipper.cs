using Ionic.Zip;
using Ionic.Zlib;
using S3ZipSharp.Interface;
using System;
using System.IO;
using System.Threading;

namespace S3ZipSharp.Services
{
    public class ObjectZipper : IObjectZipper
    {
        private readonly string _tempZipPath;
        private readonly CompressionLevel _compressionLevel;
        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        public ObjectZipper(string tempZipFileName, CompressionLevel compressionLevel)
        {
            if (String.IsNullOrEmpty(tempZipFileName))
            {
                throw new ArgumentNullException(nameof(tempZipFileName));
            }

            this._tempZipPath = tempZipFileName;
            this._compressionLevel = compressionLevel;
        }

        /// <summary>
        /// Creates a zip file in the temp directory to write compressed objects to
        /// </summary>
        public void CreateZip()
        {
            var path = Path.GetDirectoryName(_tempZipPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //Create zip archive
            //if (!File.Exists(_tempZipPath))
            //    File.Create(_tempZipPath).Close();


            using ZipFile zip = new ZipFile();
                zip.Save(_tempZipPath);
        }

        /// <summary>
        /// Compresses object and saves it into the temp zip file
        /// </summary>
        /// <param name="objectName">File name</param>
        /// <param name="data">data of the object</param>
        /// <returns></returns>
        public bool ZipObject(string objectName, Stream data)
        {
            Console.WriteLine($"Zipping object {objectName}");
            lock_.EnterWriteLock();
            using (ZipFile zip = ZipFile.Read(_tempZipPath))
            {
                zip.CompressionLevel = _compressionLevel;
              
                zip.AddEntry(objectName, data);
                zip.Save();
            }
            lock_.ExitWriteLock();
            return true;
        }

        /// <summary>
        /// Checks if the file is a valid zip file
        /// </summary>
        /// <returns></returns>
        public bool CheckZip()
        {
            return ZipFile.CheckZip(_tempZipPath);
        }
    }
}
