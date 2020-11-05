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
            var multipartContent = _fixture.GetRequestContent();

            // Act
            var result =
                await _fixture.Client.PostAsync(new Uri("/api/upload/files", UriKind.RelativeOrAbsolute), multipartContent);
            
            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            multipartContent.Headers.ContentType.MediaType.ShouldBe("multipart/form-data");
            multipartContent.Headers.ContentLength.Value.ShouldBeGreaterThan(0);
        }
    }
}