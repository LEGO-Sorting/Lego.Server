using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Lego.Server.WebApi.Dto;
using Lego.Server.WebApi.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Server.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProcessingController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public ProcessingController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VideoProcess body)
        {
            var videoProcessing = new VideoProcessing(_webHostEnvironment);
            videoProcessing.SplitVideoIntoFrames(body.ImageName, body.FramesInterval);

            Hashtable result = new Hashtable();
            result.Add("imageId", Path.GetFileNameWithoutExtension(body.ImageName));
            return Json(result);
        }
    }
}