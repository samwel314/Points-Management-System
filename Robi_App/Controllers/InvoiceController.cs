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
        private readonly IWebHostEnvironment _environment; 
        public InvoiceController(IStoreService storeService, IInvoiceService invoiceService, IAuthorizationService authorizationService , IWebHostEnvironment environment)
        {
            _storeService = storeService;
            _invoiceService = invoiceService;
            _authorizationService = authorizationService;
            _environment = environment;
        }
        // admin
        [Authorize(policy: "AdminOrEmployee")]
        public IActionResult Index(string filter = null!)
        {
            IEnumerable<ShowInvoiceVM> invoices = null!;
            if (filter == SD.zeroPoints)
            {
                invoices = _invoiceService.showInvoices(User, i => i.Points == 0);
                TempData["txt"] = "<b> فواتير لا تحتوي علي نقاط </b>";
            }
            else if (filter ==  SD.hasPoints)
            {
                invoices = _invoiceService.showInvoices(User, i => i.Points != 0);
                TempData["txt"] = "<b> فواتير تحتوي علي نقاط</b>";
            }
            else
            {
                invoices = _invoiceService.showInvoices(User);
                TempData["txt"] = "  <b>جميع الفواتير  </b>";
            }
            return View(invoices);
        }
        [Authorize(policy: "AdminOrEmployee")]

        [HttpPost]
        public async Task<IActionResult> UpdatePoints(ShowInvoiceVM model, string url = null! )
        {
            var invoicefromDB = _invoiceService.GetInvoiceToUpdate(model.Id, true); 
            if (invoicefromDB is null  )
            {
                TempData["Message"] = "! هذه الفاتورة غير موجود ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            var result = await _authorizationService.AuthorizeAsync(User, invoicefromDB, "CanUpdateInvoicePoints"); 
            if (!result.Succeeded)
                return new ForbidResult();

            if (model.Points < 0)
                return RedirectToAction("Index");

            invoicefromDB.Points = model.Points;    
            _invoiceService.Save();
            if (url != null)
                return Redirect(url); 
            //**** 
          return RedirectToAction ("Index" , new { filter  = SD.hasPoints} );  
        }
        // Client 
        [Authorize(policy: SD.Role_Client)]
        public IActionResult Create() 
        {
            CreateInvoiceVM viewModel = new CreateInvoiceVM()
            {
                Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title, 
                }).ToList(),    
            };  
            return View(viewModel);
        }
        [Authorize(policy: SD.Role_Client)]
        [HttpPost]
        public async Task<IActionResult> Create (CreateInvoiceVM model)
        {
            char? storeChar = _storeService.GetStoreChar(model.StoreId); 
            if (storeChar == null)
            {
                TempData["Message"] = "هذا الفرع غير متاح  ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            model.Code = $"{storeChar}-{model.Code}"; 
            bool isExisting = _invoiceService.IsCodeExisting(model.Code); 
            if (!ModelState.IsValid || isExisting)
            {
                model.Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title,
                }).ToList();
                if (isExisting)
                    ModelState.AddModelError("Code", "   كود الفاتورة غير صالح    ");
                return View(model);
            }
            var path = Path.Combine(_environment.WebRootPath , "InvoicesImages");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(path, fileName);
            using var stream = new FileStream(filePath , FileMode.Create); 
            await model.Image.CopyToAsync (stream);   
            // ***
            model.UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value; 
            _invoiceService.CreateInvoice(model);
            // will change it soon to UserInvoices
            return RedirectToAction("index" , "Customer"); 
        }
        [Authorize(policy: SD.Role_Client)]
        public async Task<IActionResult> Update (int Id)
        {
            var invoiceFromDB = _invoiceService.GetInvoiceToUpdate(Id , false);
            // her all users can update we will implement resource-based
            // Authorization
            if (invoiceFromDB is null     )
            {
                TempData["Message"] = "! هذه الفاتورة غير موجود ";
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
            UpdateInvoiceVM viewModel = new UpdateInvoiceVM()
            {
                Id = Id,    
                Code = invoiceFromDB.Code.Remove(0 , 2),  
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
        [Authorize(policy: SD.Role_Client)]
        [HttpPost] 
        public async Task<IActionResult> Update (UpdateInvoiceVM model)
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
                               model.Stores = _storeService.GetStores(false).Select(s => new SelectListItem()
                {
                    Value = s.Id.ToString(),
                    Text = s.Title,
                }).ToList();
                return View(model); 
            }
            var authResult = await _authorizationService
              .AuthorizeAsync(User, invoiceFromDB, "CanUpdateInvoice");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }
            char? storeChar = _storeService.GetStoreChar(model.StoreId);
            if (storeChar == null)
            {
                TempData["Message"] = "هذا الفرع غير متاح  ";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            model.Code = $"{storeChar}-{model.Code}";
            if (invoiceFromDB.Code != model.Code)
            {
                bool isExisting = _invoiceService.IsCodeExisting(model.Code);
                if (isExisting)
                {
                    ModelState.AddModelError("Code", "   كود الفاتورة غير صالح    ");
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


        [Authorize(policy: SD.Role_Admin)]

        public IActionResult ResetYear()
        {
            return View();
        }
        [Authorize(policy: SD.Role_Admin)]

        [HttpPost]
        public async Task<IActionResult> ResetYearPost()
        {
            await _invoiceService.RestYear();
            return RedirectToAction("Index", "Home"); 
        }

    }
}
