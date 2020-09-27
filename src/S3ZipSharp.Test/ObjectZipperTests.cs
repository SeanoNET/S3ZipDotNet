using NUnit.Framework;
using S3ZipSharp.Models;
using S3ZipSharp.Services;
using System;
using System.IO;
using System.IO.Compression;

namespace S3ZipSharp.Test
{
    public class ObjectZipperTests
    {
        private readonly string tempZipPath = $"{AppDomain.CurrentDomain.BaseDirectory}temp\\test.zip";
        private ObjectZipper _objectZipper;

        private S3Object GetTestObject()
        {
            byte[] file = Convert.FromBase64String("YXNmc2Rmc2Rmc2Rnc2Rnc2Rnc2Zhc2Zhc2ZzYWY=");

            return new S3Object()
            {
                Key = "test.txt",
                Content = new MemoryStream(file)
            };
        }

        [SetUp]
        public void Setup()
        {
            this._objectZipper = new ObjectZipper(tempZipPath, Ionic.Zlib.CompressionLevel.Default, null);
            _objectZipper.CreateZip();
        }

        [Test]
        public void ShouldZipObjectToTempZipFile()
        {
            var obj = GetTestObject();
            _objectZipper.ZipObject(obj.Key, obj.Content);
            obj.Content.Dispose();

            if (!File.Exists(tempZipPath))
                Assert.Fail();

            //Check size of file
            var size = new FileInfo(tempZipPath).Length;
            Assert.AreNotEqual(0, size);
      
            //Check if the file is a valid zip
            Assert.AreEqual(true, _objectZipper.CheckZip());
        }


        [Test]
        public void ShouldDeleteTempZipFileOnDispose()
        {
            using (var zipper = new ObjectZipper(tempZipPath, Ionic.Zlib.CompressionLevel.Default, null))
            {
                zipper.CreateZip();
            }

            Assert.AreEqual(false, System.IO.File.Exists(tempZipPath));
        }

        [TearDown]
        public void Cleanup()
        {
            if (System.IO.File.Exists(tempZipPath))
                File.Delete(tempZipPath);
        }
    }
}