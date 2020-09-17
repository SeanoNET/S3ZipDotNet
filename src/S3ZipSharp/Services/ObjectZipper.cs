using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp.Services
{
    class ObjectZipper
    {
        private readonly string _tempZipPath;
        

        public ObjectZipper(string tempZipFileName)
        {
            if (String.IsNullOrEmpty(tempZipFileName))
            {
                throw new ArgumentNullException(nameof(tempZipFileName));
            }

            this._tempZipPath = tempZipFileName;
        }

        public bool ZipObject(string objectName, byte[] data)
        {
            var path = Path.GetDirectoryName(_tempZipPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            

            //Create zip archive
            if (!File.Exists(_tempZipPath))
                File.Create(_tempZipPath).Close();

            using (var file = new FileStream(_tempZipPath, FileMode.Open))
            { 
                using (ZipArchive archive = new ZipArchive(file, ZipArchiveMode.Update))
                {
                    var entry = archive.CreateEntry(objectName);
                    using (var ms = new MemoryStream(data))
                    {
                        using (var zipStream = entry.Open())
                        {
                            ms.CopyTo(zipStream);
                        }
                    }
                }
            }


            return true;
        }
    }
}
