using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Lego.Server.WebApi.Dto;
using Lego.Server.WebApi.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Server.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult<IActionResult>(Ok("It works!"));
        }

        [HttpPost("info")]
        public IActionResult PostFlag([FromBody]PermissionInfo info)
        {
            return Ok(new {info.CanAccess});
        }

        [Consumes("multipart/form-data")]
        [HttpPost("files")]
        public async Task<IActionResult> Post([FromForm]List<IFormFile> file)
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
            string name = $"{Guid.NewGuid().ToString()}{extension}";

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it dose not exist.
            // var dir = new FileInfo(fileRoute);
            
            Directory.CreateDirectory(fileRoute);
            
            // Basic validation on mime types and file extension
            string[] videoMimetypes = {"video/mp4", "video/webm", "video/ogg"};
            string[] videoExt = {".mp4", ".webm", ".ogg"};

            try
            {
                // Copy contents to memory stream.
                var stream = new MemoryStream();
                theFile.CopyTo(stream);
                stream.Position = 0;
                var serverPath = link;

                // Save the file
                using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                {
                    await stream.CopyToAsync(writerFileStream);
                    writerFileStream.Dispose();
                }

                // Return the file path as json
                Hashtable videoUrl = new Hashtable();
                videoUrl.Add("imageId", Path.GetFileNameWithoutExtension(name));

                return Json(videoUrl);

                throw new ArgumentException("The video did not pass the validation");
            }

            catch (ArgumentException ex)
            {
                return Json(ex.Message);
            }
        }
    }
}