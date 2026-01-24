using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Robi_App.Models.ViewModels;
using Robi_App.Services;
using System.Security.AccessControl;
using System.Security.Claims;

namespace Robi_App.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IStoreService _storeService; 
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IStoreService storeService, IInvoiceService invoiceService)
        {
            _storeService = storeService;
            _invoiceService = invoiceService;
        }

        public IActionResult Index()
        {
            return View();
        }
        // Client 
        public IActionResult Create() 
        {
            CreateUpdateInvoiceVM viewModel = new CreateUpdateInvoiceVM()
            {
                Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title, 
                }).ToList(),    
            };  
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create (CreateUpdateInvoiceVM model)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["Message"] = "You May Not Allowed To Do This Action !";
                return RedirectToAction("Error", "Home" , new
                {
                    statusCode = 403
                });
            }
            bool isExisting = _invoiceService.IsCodeExisting(model.Code); 
            if (!ModelState.IsValid || isExisting)
            {
                model.Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title,
                }).ToList();
                if (isExisting)
                    ModelState.AddModelError("Code", "This Invoice Number Is Used Before");
                return View(model);
            }

            model.UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value; 
            _invoiceService.CreateInvoice(model);
            // will change it soon to UserInvoices
            return RedirectToAction("index" , "Customer"); 
        }

        public IActionResult Update (int Id)
        {
            var invoiceFromDB = _invoiceService.GetInvoiceToUpdate(Id , false);
            // her all users can update we will implement resource-based
            // Authorization
            if (invoiceFromDB is null || !User.Identity!.IsAuthenticated    )
            {
                TempData["Message"] = "You May Not Allowed To Do This Action !" +
                    "  This Invoice Not Found !";
                return RedirectToAction("Error", "Home" , new 
                {
                    statusCode = 404
                });
            }
            CreateUpdateInvoiceVM viewModel = new CreateUpdateInvoiceVM()
            {
                Id = Id,    
                Code = invoiceFromDB.Code,  
                UserId = invoiceFromDB.UserId ,
                StoreId = invoiceFromDB.StoreId ,
                Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title,
                }).ToList(),
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Update (CreateUpdateInvoiceVM model)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["Message"] = "You May Not Allowed To Do This Action !";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 403
                });
            }
            var invoiceFromDB = _invoiceService.GetInvoiceToUpdate(model.Id , true);

            if (!ModelState.IsValid || invoiceFromDB is null)
            {
                if (invoiceFromDB is null)
                {
                    TempData["Message"] = "This Invoice Not Found  !";
                    return RedirectToAction("Error", "Home", new
                    {
                        statusCode = 404
                    });
                }
                // in every case 
                model.Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title,
                }).ToList();
                return View(model); 
            }
            if (invoiceFromDB.Code != model.Code)
            {
                bool isExisting = _invoiceService.IsCodeExisting(model.Code);
                if (isExisting)
                {
                    ModelState.AddModelError("Code", "This Invoice Number Is Used Before");
                    model.Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                    {
                        Value = s.Id.ToString(),
                        Text = s.Title,
                    }).ToList();
                    return View(model);
                }
              
            }
            invoiceFromDB.Code = model.Code;
            invoiceFromDB.StoreId = model.StoreId;
            // to allow admin to see it agene 
            invoiceFromDB.IsReviewed = false;
            _invoiceService.Save();

            return RedirectToAction("Index" , "Customer"); 
        }

    }
}
