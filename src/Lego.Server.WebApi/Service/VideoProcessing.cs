using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using FFmpeg.AutoGen;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace Lego.Server.WebApi.Service
{
    public class VideoProcessing
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _videoRoute;
        private int _frameInterval;
        private const int DefaultFrameInterval = 20;
        public VideoProcessing(IWebHostEnvironment env)
        {
            _webHostEnvironment = env;
        }

        public void SplitVideoIntoFrames(string imageId, int framesInterval)
        {
            _frameInterval = framesInterval == default ? DefaultFrameInterval : framesInterval;
            string webRootPath = _webHostEnvironment.WebRootPath;
            _videoRoute = Path.Combine(webRootPath, $"uploads/{imageId}.mp4");
            
            // var destinationDirectoryRoute = Path.Combine(webRootPath, $"pictures/{Path.GetFileNameWithoutExtension(imageId)}");
            // Directory.CreateDirectory(destinationDirectoryRoute);

            var file = MediaFile.Open(_videoRoute);
            
            try
            {
                int i = 0;
                while (file.Video.TryReadNextFrame(out var imageData))
                {
                    if (i % _frameInterval != 0)
                    {
                        i++;
                        continue;
                    }
                    var framePixels = imageData.ToBitmap();

                    if (framePixels.Width > 1280 && framePixels.Height > 720)
                    {
                        framePixels.Mutate(x => x.Resize(framePixels.Width / 2, framePixels.Height / 2));
                    }
                    var ms = new MemoryStream();
                    framePixels.SaveAsPng(ms);
                    var frameAsPng = ms.ToArray();
                    SendPicture($"{imageId}_{i}", frameAsPng);
                    i++;
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