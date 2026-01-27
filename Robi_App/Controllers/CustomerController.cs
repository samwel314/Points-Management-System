using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Models;
using Robi_App.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Robi_App.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IInvoiceService invoiceService, UserManager<ApplicationUser> userManager)
        {
            _invoiceService = invoiceService;
            _userManager = userManager;
        }
        //Client 
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var customerData = _invoiceService.GetCustomerProfile(userId);
            return View(customerData);
        }
        // admin 

        public IActionResult Customers()
        {
            return View();      
        }
    }
}