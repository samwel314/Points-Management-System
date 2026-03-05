using Microsoft.AspNetCore.Mvc;
using Robi_App.Models.ViewModels;
using Robi_App.Services;

namespace Robi_App.Controllers
{
    public class GiftController : Controller
    {
        private readonly IGiftService _giftService;
        private readonly IWebHostEnvironment _environment;

        public GiftController(IGiftService giftService, IWebHostEnvironment environment)
        {
            _giftService = giftService;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var gifts = _giftService.GetAll();      
            return View(gifts);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateGiftVM model)
        {
            if (!ModelState.IsValid)
                return View(model); 
            if (_giftService.HaveGiftWithName(model.Name))
            {
                ModelState.AddModelError("Name", "توجد بالفعل هدية بنفس الاسم "); 
                return View(model); 
            }
            
            // -*--* work wit image 

            var path = Path.Combine(_environment.WebRootPath, "GiftsImages");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(path, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await model.Image.CopyToAsync(stream);
            model.ImagePath = fileName;  
            _giftService.CreateGift(model); 
            return RedirectToAction("Index");   
        }

        [HttpPost]
        public IActionResult Delete (int Id)
        {
            if (_giftService.DeleteGift(Id))
                return RedirectToAction("Index");
            TempData["Message"] = "! هذا الهدية غير موجودة   ";
            return RedirectToAction("Error", "Home", new
            {
                statusCode = 404
            });
        }

        public async Task<IActionResult> UpdateImage (int Id , IFormFile Image )
        {
            if (Image == null)
                return RedirectToAction("Index");
            var isUpdated = await _giftService.UpdateImage(Id, Image); 
            if (isUpdated)
                return RedirectToAction("Index");

            TempData["Message"] = "! هذا الهدية غير موجودة   ";
            return RedirectToAction("Error", "Home", new
            {
                statusCode = 404
            });
        }
    }
}
