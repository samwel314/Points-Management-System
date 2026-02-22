using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Services;

namespace Robi_App.Controllers
{
    [Authorize(policy: SD.Role_Admin)]

    public class EmployeeController : Controller
    {
        private readonly IUserService _userService;

        public EmployeeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Show()
        {
            var employees = await _userService.GetAllEmployees();
            return View(employees);
        }
    }

}
