using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace S3ZipSharp.Test
{
    class S3ClientMock
    {

        private Stream GetStream()
        {
            byte[] file = Convert.FromBase64String("YXNmc2Rmc2Rmc2Rnc2Rnc2Rnc2Zhc2Zhc2ZzYWY=");

            return new MemoryStream(file);
        }

        public AmazonS3Client GetMockedClient()
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
                               S3Objects = new List<S3Object>() { new S3Object() { Key = "test.txt" }, new S3Object() { Key = "test2.txt" } }
                           });

            return s3ClientMock.Object;
        }
    }
}
