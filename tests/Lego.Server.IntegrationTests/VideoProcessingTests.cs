using Lego.Server.IntegrationTests.Infrastructure;
using Lego.Server.WebApi;
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
    }
}