using System.IO;
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
                var file = MediaFile.Open(videoRoute);
                for (int i = 0; i < file.Video.Info.FrameCount; i++)
                {
                    var frameFileRoute = Path.Combine(destinationDirectoryRoute, $"frame_{i}.png");
                    file.Video.ReadFrame(i).ToBitmap().Save(frameFileRoute);
                }
            }
            catch(EndOfStreamException) { }
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