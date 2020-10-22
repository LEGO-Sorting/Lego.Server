using System;
using System.Threading.Tasks;

namespace Lego.Server.IntegrationTests.Infrastructure
{
    public class IntegrationTestsFixture<TStartup> : IDisposable
    {
        public async void Dispose()
        {
            await Task.CompletedTask;
        }
    }
}