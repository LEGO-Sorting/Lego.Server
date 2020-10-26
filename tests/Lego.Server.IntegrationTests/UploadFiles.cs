using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Lego.Server.IntegrationTests.Infrastructure;
using Lego.Server.WebApi;
using Lego.Server.WebApi.Dto;
using Shouldly;
using Xunit;

namespace Lego.Server.IntegrationTests
{
    public class UploadFiles : IClassFixture<IntegrationTestsFixture<Startup>>
    {
        private readonly IntegrationTestsFixture<Startup> _fixture;

        public UploadFiles(IntegrationTestsFixture<Startup> fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task UploadFiles_WhenVideoIncluded_ShouldReturn200()
        {
            // Arrange
            var body = new VideoFile()
            {
                Base64File = _fixture.ConvertVideoToBase64()
            };

            // Act
            var result =
                await _fixture.Client.PostAsJsonAsync(new Uri("/api/upload/files", UriKind.RelativeOrAbsolute), body);
            
            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}