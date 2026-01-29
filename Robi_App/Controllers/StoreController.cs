using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Robi_App.Models;
using Robi_App.Services;
using System.Collections.Immutable;

namespace Robi_App.Controllers
{
    [Authorize (policy: SD.Role_Admin)]
    public class StoreController : Controller
    {
        private readonly IStoreService _service;
        private readonly IInvoiceService _invoiceService;

        public StoreController(IStoreService service, IInvoiceService invoiceService)
        {
            _service = service;
            _invoiceService = invoiceService;
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
            _service.AddStore(store);   
            return RedirectToAction("Index");
        }

        public IActionResult Update (int id )
        {
            var store = _service.GetStoreById(id , false);
            if (store is null)
            {
                TempData["Message"] = "Sorry ! This Store Not Found";
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
                TempData["Message"] = "Sorry ! This Store Not Found";
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
                TempData["Message"] = "Sorry ! This Store Not Found";
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
                TempData["Message"] = "Sorry ! This Store Not Found";
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
            var store = _service.ShowStoreInvoicesVM    (Id);   
            if (store is null)
            {
                TempData["Message"] = "Sorry ! This Store Not Found";
                return RedirectToAction("Error", "Home" , new {});
            }
            // employee auth logic 

            store.Invoices = _invoiceService.showInvoices (i => i.StoreId == Id);
            return View(store);  
        }
    }
}
