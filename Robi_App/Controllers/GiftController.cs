using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Models.ViewModels;
using Robi_App.Services;
using System.Security.Claims;

namespace Robi_App.Controllers
{
    public class GiftController : Controller
    {
        private readonly IGiftService _giftService;
        private readonly IWebHostEnvironment _environment;
        private readonly IStoreService _storeService;

        public GiftController(IGiftService giftService, IWebHostEnvironment environment, IStoreService storeService)
        {
            _giftService = giftService;
            _environment = environment;
            _storeService = storeService;
        }

        [Authorize(policy: SD.Role_Admin)]

        public IActionResult Index()
        {
            var gifts = _giftService.GetAll();      
            return View(gifts);
        }
        [Authorize(policy: SD.Role_Admin)]

        public IActionResult Create()
        {
            return View();
        }
        [Authorize(policy: SD.Role_Admin)]

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
        [Authorize(policy: SD.Role_Admin)]

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
        [Authorize(policy: SD.Role_Admin)]

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

        [Authorize(policy: SD.Role_Admin)]

        public IActionResult UpdateName (int Id , string newName )
        {
            if (_giftService.UpdataName(Id , newName))
                return RedirectToAction("Index");
            TempData["Message"] = " \n ! هذا الهدية غير موجودة " +
                "او ربما لديك هدية بنفس الاسم    ";
            return RedirectToAction("Error", "Home", new
            {
                statusCode = 404
            });

        }
        [Authorize(policy: SD.Role_Admin)]

        public IActionResult UpdatePoints(int Id, int points)
        {
            if (_giftService.UpdataPoints(Id, points))
                return RedirectToAction("Index");
            TempData["Message"] = " \n ! هذا الهدية غير موجودة " +
                "او ربما لديك هدية بنفس الاسم    ";
            return RedirectToAction("Error", "Home", new
            {
                statusCode = 404
            });

        }

        [Authorize(policy: SD.Role_Client)]

        public IActionResult Display ()
        {
            var gifts = _giftService.GetAll();
            ViewBag.Brancehs = _storeService.GetStores(false); 
            return View(gifts);
        }
        [HttpPost]
        [Authorize(policy: SD.Role_Client)]
        public async Task<IActionResult> RequestGift (RequestGiftViewModel model)
        {
            if (model.BranchId == 0 )
            {
                TempData["Message"] = "  عند طلبك لهدية الرجاء اختيار الفرع الذي تريد استلام الهدية منه";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 400
                });
            }

            var userIid = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            model.UserId = userIid;
            if (await _giftService.IsReqestedBefore(model.GiftId , model.UserId))
            {
                TempData["Message"] = "  لقد قمت بطلب هذه الهدية من قبل الرجاء اختيار هدية أخرى ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 400
                });

            }
            var isDone = await _giftService.AddGiftRequest(model); 
            if (!isDone)
            {
                TempData["Message"] = " ! بيانات الطلب غير صحيحة ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 400
                });
            }
            return RedirectToAction("RequestSuccess", "Home"); 
 ;
        }
        [Authorize(policy: SD.Role_Client)]

        public async Task<IActionResult> ClientRequstedGifts ()
        {
            var userIid = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var giftsRequests = await _giftService.ClientRequstedGifts(userIid);    
            return View(giftsRequests); 
        }
        [HttpPost]
        [Authorize(policy: SD.Role_Client)]

        public async Task <IActionResult> DeleteGiftRequest (int id)
        {
            var userIid = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var isDeleted = await _giftService.DeleteGiftRequest(id, userIid); 
            if (!isDeleted)
            {
                TempData["Message"] = " ! هذا الطلب غير موجود  ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            return RedirectToAction("ClientRequstedGifts");
        }
    }
}
