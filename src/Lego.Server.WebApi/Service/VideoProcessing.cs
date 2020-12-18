using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.AspNetCore.Hosting;

namespace Lego.Server.WebApi.Service
{
    public class VideoProcessing
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VideoProcessing(IWebHostEnvironment env)
        {
            _webHostEnvironment = env;
        }

        public void SplitVideoIntoFrames(string imageId)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var videoRoute = Path.Combine(webRootPath, $"uploads/{imageId}");
            
            var destinationDirectoryRoute = Path.Combine(webRootPath, $"pictures/{Path.GetFileNameWithoutExtension(imageId)}");
            Directory.CreateDirectory(destinationDirectoryRoute);

            try
            {
                var file = MediaFile.Open($"{videoRoute}.mp4");
                for (int i = 0; i < file.Video.Info.FrameCount; i++)
                {
                    var frameAsBitmap = file.Video.ReadFrame(i).Data.ToArray();
                    SendPicture($"{imageId}_{i}", frameAsBitmap);
                }
                
            }
            catch(EndOfStreamException) { }
        }


        private async void SendPicture(string frameName, byte[] frameData)
        {
            var client = new HttpClient();
            var formDataContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(frameData);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var fileName = $"{frameName}.png";
            formDataContent.Add(fileContent, "image", fileName);

            var namePair = new KeyValuePair<string, string>("name", frameName);
            formDataContent.Add(new StringContent(namePair.Value), namePair.Key);

            var response = await client.PostAsync("http://127.0.0.1:5002/predict", formDataContent);

        }
    }
    
    public static class Extensions
    {
        public static Image<Bgr24> ToBitmap(this ImageData imageData)
        {
            return Image.LoadPixelData<Bgr24>(imageData.Data, imageData.ImageSize.Width, imageData.ImageSize.Height);
        }
    }

}