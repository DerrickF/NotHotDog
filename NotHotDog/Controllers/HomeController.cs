using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NotHotDog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IAmazonRekognition _rekognitionClient;

        public HomeController(IHostingEnvironment environment, IAmazonRekognition rekognitionClient)
        {
            _environment = environment;
            _rekognitionClient = rekognitionClient;
        }

        public IActionResult Index()
        {
            ViewBag.isHotDog = false;
            ViewBag.userImage = false;
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var image = new Image();
            var labels = new List<Label>();
            var isHotDot = false;
            var uploadsForlderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploadsForlderPath, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (System.IO.File.Exists(Path.Combine(uploadsForlderPath, "userUploadedImage.jpg")))
                    System.IO.File.Delete(Path.Combine(uploadsForlderPath, "userUploadedImage.jpg"));

                System.IO.File.Move(Path.Combine(uploadsForlderPath, file.FileName), Path.Combine(uploadsForlderPath, "userUploadedImage.jpg"));

                using (var fileStream = file.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    image.Bytes = ms;
                }

                DetectLabelsRequest request = new DetectLabelsRequest
                {
                    Image = image,
                    MaxLabels = 5,
                    MinConfidence = 77F
                };

                var response = await _rekognitionClient.DetectLabelsAsync(request);
                labels = response.Labels;
            }
            
            foreach (var label in labels)
            {
                if (label.Name == "Hot Dog")
                    isHotDot = true;
            }

            ViewBag.isHotDog = isHotDot;
            ViewBag.userImage = true;
            return View();
        }
    }
}
