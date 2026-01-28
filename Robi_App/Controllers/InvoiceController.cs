using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Robi_App.Models.ViewModels;
using Robi_App.Services;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Robi_App.Controllers
{

    public class InvoiceController : Controller
    {
        private readonly IStoreService _storeService; 
        private readonly IInvoiceService _invoiceService;
        private readonly IAuthorizationService _authorizationService;

        public InvoiceController(IStoreService storeService, IInvoiceService invoiceService, IAuthorizationService authorizationService)
        {
            _storeService = storeService;
            _invoiceService = invoiceService;
            _authorizationService = authorizationService;
        }
        // admin
        [Authorize(policy: SD.Role_Admin)]
        public IActionResult Index(string filter = null!)
        {
            IEnumerable<ShowInvoiceVM> invoices = null!;
            if (filter == SD.zeroPoints)
            {
                invoices = _invoiceService.showInvoices(i => i.Points == 0);
                TempData["txt"] = "<b> فواتير لا تحتوي علي نقاط </b>";
            }
            else if (filter ==  SD.hasPoints)
            {
                invoices = _invoiceService.showInvoices(i => i.Points != 0);
                TempData["txt"] = "<b> فواتير تحتوي علي نقاط</b>";
            }
            else
            {
                invoices = _invoiceService.showInvoices();
                TempData["txt"] = "  <b>جميع الفواتير  </b>";
            }
            return View(invoices);
        }
        [Authorize(policy: SD.Role_Admin)]
        public IActionResult UpdatePoints(ShowInvoiceVM model)
        {
            var invoicefromDB = _invoiceService.GetInvoiceToUpdate(model.Id, true); 
            if (invoicefromDB is null  )
            {
                TempData["Message"] = "This Invoice Not Found !";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            if (model.Points < 0)
                return RedirectToAction("Index");

            invoicefromDB.Points = model.Points;

            // if admin set it with 0 points allow him to show it agene with zero invoices 
            if (model.Points != 0)
                invoicefromDB.IsReviewed = true;
            else
                invoicefromDB.IsReviewed = false;   
                _invoiceService.Save(); 
            if (model.CustomerId != null)
                return RedirectToAction("Show" , "Customer" , new { Id = model.CustomerId });
            return RedirectToAction ("Index");  
        }
        // Client 
        //[Authorize(policy: SD.Role_Client)]

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

        public async Task<IActionResult> Update (int Id)
        {
            var invoiceFromDB = _invoiceService.GetInvoiceToUpdate(Id , false);
            // her all users can update we will implement resource-based
            // Authorization
            if (invoiceFromDB is null     )
            {
                TempData["Message"] = 
                    "This Invoice Not Found !";
                return RedirectToAction("Error", "Home" , new 
                {
                    statusCode = 404
                });
            }

            var authResult = await _authorizationService
                .AuthorizeAsync(User, invoiceFromDB, "CanUpdateInvoice");
            if (!authResult.Succeeded)
            {
                return new ForbidResult(); 
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
        public async Task<IActionResult> Update (CreateUpdateInvoiceVM model)
        {
            
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
                var authResult = await _authorizationService
               .AuthorizeAsync(User, invoiceFromDB, "CanUpdateInvoice");
                if (!authResult.Succeeded)
                {
                    return new ForbidResult();
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
