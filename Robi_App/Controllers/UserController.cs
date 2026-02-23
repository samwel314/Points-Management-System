using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Models;
using Robi_App.Services;
using System.Formats.Asn1;

namespace Robi_App.Controllers
{
    [Authorize(policy: SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Lock(string id , string url)
        {
            var user = await _userManager.FindByIdAsync(id);  
            if (user == null)
            {
                TempData["Message"] = "هذا الحساب غير موجود";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
           await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(10));
            if (url != null)
                return Redirect(url); 

             return RedirectToAction("index" , "home");
        }

        public async Task<IActionResult> UnLock(string id, string url)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Message"] = "هذا الحساب غير موجود";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);
            if (url != null)
                return Redirect(url);

            return RedirectToAction("index", "home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete (string id , string url )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Message"] = "هذا الحساب غير موجود";
                return RedirectToAction("Error", "Home", new
                {
                    statusCode = 404
                });
            }
           await  _userManager.DeleteAsync(user); 
            if (url != null)
                return Redirect(url);
            return RedirectToAction("index", "home");
        }

    }
}
