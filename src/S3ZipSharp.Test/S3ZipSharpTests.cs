using NUnit.Framework;
using S3ZipSharp.Interface;
using S3ZipSharp.Models;
using S3ZipSharp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace S3ZipSharp.Test
{
    class S3ZipSharpTests
    {

        [Test]
        public void ThrowsOnMissingS3BucketInConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() => new Config("", "", "",""));
        }

        [Test]
        public void ShouldHaveDefaultConfigValues()
        {
            var config = new Models.Config("","","","test");

            Assert.IsTrue(config.TempZipDir.Contains($"{System.IO.Path.GetTempPath()}\\S3ZipSharp\\"));
            Assert.IsTrue(config.TempZipPath.Contains(@"tmp"));
            Assert.AreEqual(CompressionLevel.Default, config.CompressionLevel);
        }
    }
}
