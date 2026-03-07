using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Robi_App.Models;
using Robi_App.Services;
using Robi_App.Services.Implementation;
using System.Collections.Immutable;

namespace Robi_App.Controllers
{
    [Authorize (policy: SD.Role_Admin)]
    public class StoreController : Controller
    {
        private readonly IStoreService _service;
        private readonly IInvoiceService _invoiceService;
        private readonly IGiftService _giftService;

        public StoreController(IStoreService service, IInvoiceService invoiceService, IGiftService giftService)
        {
            _service = service;
            _invoiceService = invoiceService;
            _giftService = giftService;
        }

        public IActionResult Index()
        {
            var Stores = _service.GetStores(false);
            return View(Stores);
        }

        public IActionResult Add()
        {
            return View();
        }   
        [HttpPost]
        public IActionResult Add (Store store )
        {
             if(!ModelState.IsValid)
                return View(store);     
            if (store.Id != 0)
            {
                TempData["Message"] = "Some Error With Data !";
                return RedirectToAction("Error", "Home" , new
                {
                    statusCode = 400
                });
            } 
            store.S_char = GetNextChar(); 
            _service.AddStore(store);   
            return RedirectToAction("Index");
        }

        public IActionResult Update (int id )
        {
            var store = _service.GetStoreById(id , false);
            if (store is null)
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }

            return View(store); 
        }
        [HttpPost]
        public IActionResult Update (Store store )
        {
            if (!ModelState.IsValid )
               return View(store);  
            
            if ( !_service.StoreExists(store.Id))
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home" ,  new
                {
                    statusCode = 404
                });
            }
            _service.UpdateStore(store);    
            return RedirectToAction("Index");   
        }

        public IActionResult Delete (int id)
        {
            var store = _service.GetStoreById(id, false);
            if (store is null)
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            return View(store); 
        }

        [HttpPost]
        public IActionResult Delete(Store store)
        {
     
        
            if (!_service.StoreExists(store.Id))
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            _service.DeleteStore(store);
            return RedirectToAction("Index");
        }

        public IActionResult ShowInvoices (int Id)
        {
            var store = _service.ShowStoreInvoicesVM(Id);   
            if (store is null)
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home" , new {});
            }

            store.Invoices = _invoiceService.showInvoices (null!,  i => i.StoreId == Id);
            return View(store);  
        }

        public async Task<IActionResult> Gifts (int Id)
        {
             var store = await _service.ShowStoreGifts (Id);
            if (store is null)
            {
                TempData["Message"] = "هذا الفرع غير موجود ";
                return RedirectToAction("Error", "Home", new { });
            }
            return View(store);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleAvailability (int id , string Url )
        {
            var IsChanged = await _giftService.ChangeAvailability(id); 
            if (!IsChanged)
            {
                TempData["Message"] = "هذا الطلب غير موجود ";
                return RedirectToAction("Error", "Home", new { });
            }
            if (Url is null)
              return  RedirectToAction("Index");

            return Redirect(Url); 
        }

        public async Task<IActionResult> DeleteGiftRequest (int Id  , string Url )
        {
            var IsDeleted = await _giftService.DeleteGiftRequest(Id);
            if (!IsDeleted)
            {
                TempData["Message"] = "هذا الطلب غير موجود ";
                return RedirectToAction("Error", "Home", new { });
            }
            if (Url is null)
                return RedirectToAction("Index");

            return Redirect(Url);
        }
        private char GetNextChar ()
        {
            char S_char =  ' ';
            var cahrsFromDB = _service.GetStores(false).Select(s => s.S_char);
            for (int i =  65;  i <= 90; i++  )
            {
                if (!cahrsFromDB.Any(c => c == (char)i))
                {
                    S_char = (char)i;
                    break;
                }
            }
            return S_char;  
        }
    }

}
