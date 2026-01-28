using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Models;
using Robi_App.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Robi_App.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService; 
        public CustomerController(IInvoiceService invoiceService,
            UserManager<ApplicationUser> userManager , IUserService userService)
        {
            _invoiceService = invoiceService;
            _userManager = userManager;
            _userService = userService;
        }
        //Client 
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var customerData = _invoiceService.GetCustomerProfile(userId);
            return View(customerData);
        }
        // admin 
        [Authorize(policy: SD.Role_Admin)]

        public async Task<IActionResult> Customers()
        {
            var Customers = await _userService.GetAllCustomers(); 
            return View(Customers);      
        }
        [Authorize(policy: SD.Role_Admin)]

        public IActionResult Show(string Id)
        {
            var customer =  _invoiceService.GetCustomerProfile(Id);
            return View(customer);      
        }
    }
}