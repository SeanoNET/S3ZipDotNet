using Ionic.Zip;
using S3ZipSharp.Interface;
using System;
using System.IO;
using System.Threading;

namespace S3ZipSharp.Services
{
    internal class ObjectZipper : IObjectZipper
    {
        private readonly string _tempZipPath;
        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        public ObjectZipper(string tempZipFileName)
        {
            if (String.IsNullOrEmpty(tempZipFileName))
            {
                throw new ArgumentNullException(nameof(tempZipFileName));
            }

            this._tempZipPath = tempZipFileName;

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
            if (!File.Exists(_tempZipPath))
                File.Create(_tempZipPath).Close();


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
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;

                zip.AddEntry(objectName, data);
                zip.Save();
            }
            lock_.ExitWriteLock();
            return true;
        }
    }
}
