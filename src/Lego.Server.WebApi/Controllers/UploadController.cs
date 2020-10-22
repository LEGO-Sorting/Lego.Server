using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Server.WebApi.Controllers
{
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("UploadFiles")]
        [Produces("application/json")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            // Get the file from the POST request
            var theFile = HttpContext.Request.Form.Files.GetFile("file");

            // Get the server path, wwwroot
            string webRootPath = _webHostEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath, "uploads");

            // Get the mime type
            var mimeType = HttpContext.Request.Form.Files.GetFile("file").ContentType;

            // Get File Extension
            string extension = System.IO.Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it dose not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] videoMimetypes = {"video/mp4", "video/webm", "video/ogg"};
            string[] videoExt = {".mp4", ".webm", ".ogg"};

            try
            {
                if (Array.IndexOf(videoMimetypes, mimeType) >= 0 && (Array.IndexOf(videoExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                        await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }

                    // Return the file path as json
                    Hashtable videoUrl = new Hashtable();
                    videoUrl.Add("link", "/uploads/" + name);

                    return Json(videoUrl);
                }

                throw new ArgumentException("The video did not pass the validation");
            }

            catch (ArgumentException ex)
            {
                return Json(ex.Message);
            }
        }
    }
}