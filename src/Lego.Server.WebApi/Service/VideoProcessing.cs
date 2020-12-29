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

namespace Lego.Server.WebApi.Service
{
    public class VideoProcessing
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _videoRoute;
        private int _frameInterval = 15;
        public VideoProcessing(IWebHostEnvironment env)
        {
            _webHostEnvironment = env;
        }

        public void SplitVideoIntoFrames(string imageId)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            _videoRoute = Path.Combine(webRootPath, $"uploads/{imageId}.mp4");
            
            // var destinationDirectoryRoute = Path.Combine(webRootPath, $"pictures/{Path.GetFileNameWithoutExtension(imageId)}");
            // Directory.CreateDirectory(destinationDirectoryRoute);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var currentDirectory = $"{Environment.CurrentDirectory}\\bin\\Debug\\net5.0\\ffmpeg\\";
                FFmpegLoader.FFmpegPath = currentDirectory;
            }
            
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