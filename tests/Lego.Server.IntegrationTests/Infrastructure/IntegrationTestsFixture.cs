using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Lego.Server.IntegrationTests.Infrastructure
{
    public class IntegrationTestsFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        private string VideoPath = Path.Combine(System.IO.Path.GetFullPath(@"../../../"), @"Resources/cup-01.mp4");
        private string UploadedVideoPath = Path.Combine(System.IO.Path.GetFullPath(@"../../../../../"), @"src/Lego.Server.WebApi/wwwroot/uploads/");
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            Client = CreateClient();
        }

        public string ConvertVideoToBase64()
        {
            var bytes = File.ReadAllBytes(VideoPath);
            return Convert.ToBase64String(bytes);
        }
        
        protected override void Dispose(bool disposing)
        {
            var di = new DirectoryInfo(UploadedVideoPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete(); 
            }
            base.Dispose(disposing);
        }
    }
}