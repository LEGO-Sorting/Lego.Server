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
    public class VideoProcessingTests : IClassFixture<IntegrationTestsFixture<Startup>>
    {
        private readonly IntegrationTestsFixture<Startup> _fixture;

        public VideoProcessingTests(IntegrationTestsFixture<Startup> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task VideoProcessing_WhenVideoExists_ShouldReturn200()
        {
            // Arrange
            _fixture.FakeUploadVideo();
            var body = new VideoProcess()
            {
                ImageName = _fixture.VideoFileName
            };
            
            // Act
            var result =
                await _fixture.Client.PostAsJsonAsync(new Uri($"/api/processing/", UriKind.RelativeOrAbsolute), body);
            
            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}