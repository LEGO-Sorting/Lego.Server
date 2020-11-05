using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Lego.Server.IntegrationTests.Infrastructure;
using Lego.Server.WebApi;
using Lego.Server.WebApi.Dto;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace Lego.Server.IntegrationTests
{
    public class UploadFilesTests : IClassFixture<IntegrationTestsFixture<Startup>>
    {
        private readonly IntegrationTestsFixture<Startup> _fixture;

        public UploadFilesTests(IntegrationTestsFixture<Startup> fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task UploadFiles_WhenVideoIncluded_ShouldReturn200()
        {
            // Arrange
            var stream = File.OpenRead(_fixture.VideoPath);
            var videoFile = new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(_fixture.VideoPath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(videoFile.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = videoFile.Length,
                    ContentType = new MediaTypeHeaderValue(videoFile.ContentType)
                },
            }, "file", videoFile.FileName);

            // Act
            var result =
                await _fixture.Client.PostAsync(new Uri("/api/upload/files", UriKind.RelativeOrAbsolute), multipartContent);
            
            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}