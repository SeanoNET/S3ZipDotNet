﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using NUnit.Framework;
using S3ZipSharp.Interface;
using S3ZipSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S3ZipSharp.Test
{
    class S3ClientProxyTests
    {
        IFileRetriever s3Proxy;

        public S3ClientProxyTests()
        {
            var s3ClientMock = new Mock<AmazonS3Client>(FallbackCredentialsFactory.GetCredentials(true), new AmazonS3Config { RegionEndpoint = RegionEndpoint.APSoutheast2 });

            s3ClientMock
                        .Setup(x => x.GetObjectAsync(
                           It.IsAny<GetObjectRequest>(),
                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(
                           (GetObjectRequest request, CancellationToken ct) =>
                             new GetObjectResponse
                             {
                                 Key = request.Key,
                                 HttpStatusCode = HttpStatusCode.OK,
                                 ResponseStream = GetStream(),
                             });

            s3ClientMock
                      .Setup(x => x.ListObjectsV2Async(
                         It.IsAny<ListObjectsV2Request>(),
                         It.IsAny<CancellationToken>()))
                      .ReturnsAsync(
                         (ListObjectsV2Request request, CancellationToken ct) =>
                           new ListObjectsV2Response
                           {
                               NextContinuationToken = null,
                               S3Objects = new List<S3Object>() { new S3Object() { Key = "test.txt"}, new S3Object() { Key = "test2.txt" } }
                           });

            

            s3Proxy = new S3ClientProxy(s3ClientMock.Object, 10);
        }

        private Stream GetStream()
        {
            byte[] file = Convert.FromBase64String("YXNmc2Rmc2Rmc2Rnc2Rnc2Rnc2Zhc2Zhc2ZzYWY=");

            return new MemoryStream(file);
        }

        [SetUp]
        public void Setup()
        {
       
        }

        [Test]
        public async Task ShouldGet2ObjectsInListObjects()
        {
            List<string> result = new List<string>();

            await foreach(var keys in s3Proxy.ListObjectsAsStream("", "", new CancellationToken()))
            {
                 result.AddRange(keys);
            }
  
            Assert.AreEqual(2, result.Count);

        }
        [Test]
        public async Task ShouldGet2ObjectsFromFetchObjectsAsStream()
        {
           
            List<string> lsKeys = new List<string>() { "test.txt", "test2.txt" };
            List<Models.S3Object> result = new List<Models.S3Object>();
            await foreach (var obj in s3Proxy.FetchObjectsAsStream("", lsKeys, new CancellationToken()))
            {
                result.Add(obj);
            }

            Assert.AreEqual(2, result.Count);

            for (int i = 0; i < result.Count -1; i++)
            {
                Assert.AreEqual(lsKeys[i], result[i].Key);
                Assert.NotNull(result[i].Content);
            }
        }



    }
}
