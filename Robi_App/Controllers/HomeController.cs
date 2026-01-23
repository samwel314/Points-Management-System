using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Robi_App.Models;

namespace Robi_App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
