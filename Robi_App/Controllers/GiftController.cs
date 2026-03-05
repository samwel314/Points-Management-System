using Microsoft.AspNetCore.Mvc;

namespace Robi_App.Controllers
{
    public class GiftController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
